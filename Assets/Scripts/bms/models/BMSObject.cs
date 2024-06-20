using System.Collections.Generic;

namespace BMS
{
	public class Line
	{
		public List<Note> NoteList;
		public List<Note> LandMineList;
		public Line()
		{
			NoteList = new List<Note>()
			{
				Capacity = 225
			};
			LandMineList = new List<Note>()
			{
				Capacity = 20
			};
		}
	}


	public abstract class BMSObject: System.IComparable<BMSObject>
	{
		public int Bar { get; protected set; }
		public double Beat { get; protected set; }
		public double Timing { get; set; } 

		public BMSObject(int bar, double beat, double beatLength)
		{
			Bar = bar;
			Beat = (beat / beatLength) * 4.0f;
		}

		public BMSObject(int bar, double beat)
		{
			Bar = bar;
			Beat = beat;
		}

		public void CalculateBeat(double prevBeats, double measureLength)
		{
			Beat = Beat * measureLength + prevBeats;
		}
		
		public int CompareTo(BMSObject other)
		{
			if (Beat < other.Beat) return -1;
			if (Beat == other.Beat) return 0;
			return 1;
		}
	}
}
