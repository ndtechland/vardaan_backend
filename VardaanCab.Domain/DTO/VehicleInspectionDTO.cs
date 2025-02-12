using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class VehicleInspectionDTO
    {
        public int Id { get; set; }
        public Nullable<int> Vendor_Id { get; set; }
        public Nullable<int> Vehicle_Id { get; set; }
        public Nullable<System.DateTime> InspectionDate { get; set; }
        public bool AC_Working { get; set; }
        public string AC_Remarks { get; set; }
        public bool UnderInfluence { get; set; }
        public string UnderInfluence_Remarks { get; set; }
        public bool Wiper_Seasonal { get; set; }
        public string Wiper_Remarks { get; set; }
        public bool National_Permit { get; set; }
        public string NationalPermit_Remarks { get; set; }
        public bool Windshield_Broken { get; set; }
        public string Windshield_Remarks { get; set; }
        public int Trip_Type { get; set; }
        public bool Visible_Body_Dent { get; set; }
        public string BodyDent_Remarks { get; set; }
        public bool Seat_Belts_Working { get; set; }
        public string SeatBelts_Remarks { get; set; }
        public bool GPS_Not_Available { get; set; }
        public string GPS_Remarks { get; set; }
        public bool State_Permit { get; set; }
        public string StatePermit_Remarks { get; set; }
        public bool Unregistered_Drivers { get; set; }
        public string UnregisteredDrivers_Remarks { get; set; }
        public int Shift_Time { get; set; }
        public bool Dirty_Unclean_Vehicle { get; set; }
        public string UncleanVehicle_Remarks { get; set; }
        public bool Seat_Cover { get; set; }
        public string SeatCover_Remarks { get; set; }
        public bool Headlights_Indicators { get; set; }
        public string Headlights_Remarks { get; set; }
        public bool Insurance { get; set; }
        public string Insurance_Remarks { get; set; }
        public bool Unregistered_Cab { get; set; }
        public string UnregisteredCab_Remarks { get; set; }
        public string City_Name { get; set; }
        public bool Driver_Uniform { get; set; }
        public string DriverUniform_Remarks { get; set; }
        public bool Spare_Wheel { get; set; }
        public string SpareWheel_Remarks { get; set; }
        public bool RC_Book { get; set; }
        public string RCBook_Remarks { get; set; }
        public bool Pollution { get; set; }
        public string Pollution_Remarks { get; set; }
        public Nullable<decimal> Penalty_Amount { get; set; }
        public string Feedback { get; set; }
        public bool Fire_Extinguisher { get; set; }
        public string FireExtinguisher_Remarks { get; set; }
        public bool Tool_Kit { get; set; }
        public string ToolKit_Remarks { get; set; }
        public bool Fitness { get; set; }
        public string Fitness_Remarks { get; set; }
        public bool Commercial_License { get; set; }
        public string CommercialLicense_Remarks { get; set; }
        public string Penalty_Description { get; set; }
        public bool First_Aid_Box { get; set; }
        public string FirstAidBox_Remarks { get; set; }
        public bool Fog_Lamp { get; set; }
        public string FogLamp_Remarks { get; set; }
        public bool Passenger_Tax { get; set; }
        public string PassengerTax_Remarks { get; set; }
        public bool Vehicle_Model_Over_5_Years { get; set; }
        public string VehicleModel_Remarks { get; set; }
        public Nullable<int> Total_NC_Count { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> InspectionByEmployeeId { get; set; }
        public SelectList Companies { get; set; }
        public SelectList TripTypes { get; set; }
        public SelectList DropshiftTimes { get; set; }
        public SelectList PickUpshiftTimes { get; set; }
    }
}
