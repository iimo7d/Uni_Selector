namespace Uni_Selector.ViewModels.UserManagement
{
    public class UserListPageViewModel
    {
        public List<UserListViewModel> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalUsers { get; set; }
        public string SearchTerm { get; set; }
        public string RoleFilter { get; set; }
        public string StatusFilter { get; set; }
        public List<string> AvailableRoles { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public int StartRecord => (CurrentPage - 1) * PageSize + 1;
        public int EndRecord => Math.Min(CurrentPage * PageSize, TotalUsers);
    }
}
