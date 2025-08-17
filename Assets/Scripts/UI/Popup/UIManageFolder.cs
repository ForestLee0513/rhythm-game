using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;
using BMSParser;

public class UIManageFolder : UIPopup
{
    enum Texts
    {

    }

    enum Objects
    {
        FolderListContent
    }

    enum Buttons
    {
        OpenFolder,
        RemoveFolder,
        BuildFolderDB
    }

    enum Images
    {
        Image // Dummy Spinning image..
    }

    // error cases
    // bpm 중복?
    // 확장자 대문자로 인한 오류
    // 6K U_E FULL PACK 1.13/eternal_drain_6KSH.bms: Object reference not set to an instance of an object // 마디가 넘어간 LNOBJ 롱노트에 대한 예외처리가 안되어 있었음. 예) 73번째 마디 0.75 -> 74번째 마디 0

    List<FolderInfo.Model> folders;
    List<int> selectedFolderIndex = new();
    private bool isProcessing = false;
    private CancellationTokenSource cancellationTokenSource;

    // 정적 변수로 앱 종료 상태 추적
    private static bool isApplicationQuitting = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));

        GetButton((int)Buttons.OpenFolder).gameObject.BindEvent(OpenFolder);
        GetButton((int)Buttons.RemoveFolder).gameObject.BindEvent(RemoveFolder);
        GetButton((int)Buttons.BuildFolderDB).gameObject.BindEvent(BuildSongDB);

        RefreshFolders();

        OnProgressUpdate += (progress, message) => {
            Debug.Log($"Importing files Progress: {progress:P1} - {message}");
        };

        OnScanComplete += (files) => {
            Debug.Log($"Scan completed! Found {files.Count} BMS files.");
            ProcessBMSFiles(files);
        };

        return true;
    }

    private void Update()
    {
        GetImage((int)Images.Image).transform.Rotate(Vector2.down, 1000 * Time.deltaTime);
    }

    private void RefreshFolders()
    {
        // clear example ui
        Transform parent = GetObject((int)Objects.FolderListContent).gameObject.transform;

        foreach (Transform t in parent)
        {
            Managers.Resource.Destroy(t.gameObject);
        }

        // append ui
        folders?.Clear();
        folders = Managers.SongInfoDataManager.ReadTable<FolderInfo.Model>(FolderInfo.Table.Name);

        foreach (FolderInfo.Model folderItem in folders)
        {
            var folderItemObject = Managers.UI.MakeSubItem<UI_FolderListItem>(parent.transform);
            folderItemObject.SetInfo(folderItem.FOLDER_NAME, folderItem.PATH);
        }
    }

    private void OpenFolder()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("폴더 선택", "", false);

        if (paths.Length == 0)
        {
            Debug.Log("user cancel");
            return;
        }

        string selectedPath = paths[0];

        List<FolderInfo.Model> existFolderPaths = Managers.SongInfoDataManager.SearchFolderPath<FolderInfo.Model>(selectedPath);

        if (existFolderPaths.Count > 0)
        {
            Debug.Log("경로가 이미 존재함");
            return;
        }

        if (Managers.SongInfoDataManager.PushFolder(selectedPath))
        {
            Debug.Log("파일 추가 완료");
            RefreshFolders();
        }
    }

    private void RemoveFolder()
    {
        if (selectedFolderIndex.Count == 0)
            return;

        Debug.Log("remove folder");
    }

    private readonly string[] bmsExtensions = { ".bms", ".bme", ".pms", ".bml" };

    public event Action<float, string> OnProgressUpdate;
    public event Action<List<string>> OnScanComplete;

    private async void BuildSongDB()
    {
        // 앱 종료 중이면 실행하지 않음
        if (isApplicationQuitting)
        {
            Debug.Log("Application is quitting, skipping BMS scan");
            return;
        }

        if (isProcessing)
        {
            Debug.Log("Already Processing...");
            return;
        }

        Debug.Log("Build Songs DB...");
        isProcessing = true;

        try
        {
            // 기존 작업 안전하게 취소
            await CancelCurrentOperationSafely();

            cancellationTokenSource = new CancellationTokenSource();

            var bmsFiles = await ScanFoldersAsync(folders, cancellationTokenSource.Token);

            // 취소되지 않았고 앱이 종료되지 않았을 때만 처리
            if (!cancellationTokenSource.Token.IsCancellationRequested && !isApplicationQuitting)
            {
                OnScanComplete?.Invoke(bmsFiles);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("BMS scan was cancelled.");
        }
        catch (Exception ex)
        {
            // 앱 종료로 인한 오류는 무시
            if (!isApplicationQuitting)
            {
                Debug.LogError($"Error building song DB: {ex.Message}");
            }
        }
        finally
        {
            isProcessing = false;
        }
    }

    // 안전한 작업 취소
    private async Task CancelCurrentOperationSafely()
    {
        if (cancellationTokenSource != null)
        {
            try
            {
                cancellationTokenSource.Cancel();

                // 취소 완료까지 잠시 대기 (최대 1초)
                var waitTime = 0;
                while (isProcessing && waitTime < 1000)
                {
                    await Task.Delay(50);
                    waitTime += 50;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error during cancellation: {ex.Message}");
            }
            finally
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }
    }

    public async void CancelScan()
    {
        await CancelCurrentOperationSafely();
        isProcessing = false;
    }

    private async Task<List<string>> ScanFoldersAsync(List<FolderInfo.Model> folders, CancellationToken cancellationToken = default)
    {
        var allFiles = new List<string>();

        for (int i = 0; i < folders.Count; i++)
        {
            // 취소 또는 앱 종료 확인
            if (cancellationToken.IsCancellationRequested || isApplicationQuitting)
                break;

            var folderPath = folders[i].PATH;
            OnProgressUpdate?.Invoke((float)i / folders.Count, $"Scanning BMS folders...: {Path.GetFileName(folderPath)}");

            try
            {
                var files = await Task.Run(() => ScanSingleFolder(folderPath, cancellationToken), cancellationToken);
                allFiles.AddRange(files);
            }
            catch (OperationCanceledException)
            {
                // 취소는 정상적인 상황
                break;
            }
            catch (Exception ex)
            {
                // 앱 종료로 인한 오류가 아닌 경우에만 로그
                if (!isApplicationQuitting && !ex.Message.Contains("Thread was being aborted"))
                {
                    Debug.LogError($"Error scanning folder {folderPath}: {ex.Message}");
                }
            }

            await Task.Yield();
        }

        return allFiles;
    }

    private List<string> ScanSingleFolder(string folderPath, CancellationToken cancellationToken = default)
    {
        // 앱 종료 또는 취소 확인
        if (isApplicationQuitting || cancellationToken.IsCancellationRequested)
            return new List<string>();

        if (!Directory.Exists(folderPath))
            return new List<string>();

        try
        {
            var files = new List<string>();

            // Directory.EnumerateFiles를 더 안전하게 처리
            foreach (var file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                // 매 파일마다 취소 확인
                if (cancellationToken.IsCancellationRequested || isApplicationQuitting)
                    break;

                if (bmsExtensions.Any(ext => file.ToLower().EndsWith(ext)))
                {
                    files.Add(file);
                }
            }

            return files;
        }
        catch (OperationCanceledException)
        {
            return new List<string>();
        }
        catch (Exception ex)
        {
            // Thread abort 오류나 앱 종료 관련 오류는 무시
            if (!isApplicationQuitting &&
                !ex.Message.Contains("Thread was being aborted") &&
                !ex.Message.Contains("being finalized"))
            {
                Debug.LogError($"Error enumerating files in {folderPath}: {ex.Message}");
            }

            return new List<string>();
        }
    }

    private async void ProcessBMSFiles(List<string> bmsFiles)
    {
        const int processBatchSize = 10;

        try
        {
            for (int i = 0; i < bmsFiles.Count; i += processBatchSize)
            {
                // 앱 종료 또는 취소 확인
                if (isApplicationQuitting || cancellationTokenSource?.Token.IsCancellationRequested == true)
                    break;

                var batch = bmsFiles.Skip(i).Take(processBatchSize);

                foreach (var file in batch)
                {
                    try
                    {
                        await ProcessSingleBMSFile(file);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!isApplicationQuitting)
                        {
                            Debug.LogError($"Error processing {file}: {ex.Message}");
                        }
                    }
                }

                if (!isApplicationQuitting)
                {
                    OnProgressUpdate?.Invoke((float)i / bmsFiles.Count, $"Processing BMS files... {i}/{bmsFiles.Count}");
                }

                await Task.Yield();
            }

            if (!isApplicationQuitting)
            {
                Debug.Log("BMS processing completed!");
            }
        }
        finally
        {
            isProcessing = false;
        }
    }

    private BMS parser = new BMS();

    private async Task ProcessSingleBMSFile(string filePath)
    {
        // 앱 종료 중이면 스킵
        if (isApplicationQuitting)
            return;

        try
        {
            await Task.Run(() => {
                // 파서 실행 전 한 번 더 확인
                if (isApplicationQuitting || cancellationTokenSource?.Token.IsCancellationRequested == true)
                    return;

                var bmsModel = parser.Decode(filePath);

                // 파서 실행 후에도 확인
                if (!isApplicationQuitting)
                {
                    Debug.Log($"Parsed: {bmsModel.Title}");
                }
            }, cancellationTokenSource?.Token ?? CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // 정상적인 취소
        }
        catch (Exception ex)
        {
            if (!isApplicationQuitting)
            {
                Debug.LogError($"Error parsing {filePath}: {ex.Message}");
            }
        }
    }

    // 앱 종료 감지 개선
    private void OnApplicationQuit()
    {
        Debug.Log("Application quitting - cleaning up BMS parsing...");
        isApplicationQuitting = true;

        // 비동기 정리 작업을 동기로 처리
        CleanupSynchronously();
    }

    private void CleanupSynchronously()
    {
        try
        {
            // 취소 토큰 즉시 취소
            cancellationTokenSource?.Cancel();

            // 잠시 대기하여 스레드들이 정리될 시간 확보
            System.Threading.Thread.Sleep(100);

            // 리소스 정리
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;

            Debug.Log("Cleanup completed successfully");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error during cleanup: {ex.Message}");
        }
        finally
        {
            isProcessing = false;
        }
    }

    // 컴포넌트 파괴 시에도 정리
    private void OnDestroy()
    {
        if (!isApplicationQuitting)
        {
            isApplicationQuitting = true;
            CleanupSynchronously();
        }
    }
}
