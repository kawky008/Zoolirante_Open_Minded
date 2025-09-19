namespace Zoolirante_Open_Minded.ViewModels
{
	public class HomeIndexViewModel
	{
		public List<EventViewModel> EventsToday { get; set; } = new();
		public List<EventViewModel> EventsUpcoming { get; set; } = new();
	}

	public class EventViewModel
	{
		public string Title { get; set; }
		public string Location { get; set; }
	}
}
