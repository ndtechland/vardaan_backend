using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class CreateRequestDTO
    {
        public int Id { get; set; }
        public string RequestType { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string GuestContact { get; set; }
        public string Unit { get; set; }
        public string Department { get; set; }
        public string CostCentre { get; set; }
        public string ExpenseCode { get; set; }
        public string RequestorEmpId { get; set; }
        public string RequestorName { get; set; }
        public string RequestorContacts { get; set; }
        public Nullable<System.DateTime> BookingReceivedDate { get; set; }
        public string RequestTripType { get; set; }
        public string DestinationRequestMethod { get; set; }
        public string LocationType { get; set; }
        public Nullable<System.DateTime> StartRequestDate { get; set; }
        public Nullable<System.DateTime> EndRequestDate { get; set; }
        public Nullable<int> TripType { get; set; }
        public Nullable<int> ShiftType { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public int MenuId { get; set; }
        public Nullable<int> PickupShiftTimeId { get; set; }
        public Nullable<int> DropShiftTimeId { get; set; }
        public string SMSTriggeredLocation { get; set; }
        public SelectList Companies { get; set; }
        public SelectList TripTypes { get; set; }
        public SelectList PickUpshiftTimes { get; set; }
        public SelectList DropshiftTimes { get; set; }
        public SelectList ShiftTypes { get; set; }
        public IEnumerable<EmployeeRequests> EmployeeRequestList { get; set; }
    }
    public class EmployeeRequests
    {
        public int Id { get; set; }
        public string RequestType { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string EmployeeId { get; set; }
        public int Employee_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string GuestContact { get; set; }
        public string Unit { get; set; }
        public string Department { get; set; }
        public string CostCentre { get; set; }
        public string ExpenseCode { get; set; }
        public string RequestorEmpId { get; set; }
        public string RequestorName { get; set; }
        public string RequestorContacts { get; set; }
        public string OrgName { get; set; }
        public Nullable<System.DateTime> BookingReceivedDate { get; set; }
        public string RequestTripType { get; set; }
        public string DestinationRequestMethod { get; set; }
        public string LocationType { get; set; }
        public Nullable<System.DateTime> StartRequestDate { get; set; }
        public Nullable<System.DateTime> EndRequestDate { get; set; }
        public string TripType { get; set; }
        public string ShiftType { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public int MenuId { get; set; }
        public string SMSTriggeredLocation { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
