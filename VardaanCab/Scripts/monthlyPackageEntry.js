/// <reference path="jquery-1.12.4.js" />
//let oldFare = $("#oldFare").val();
//let startingMinHour = parseInt($("#MinHrs").val());
//let startingMinKm = parseInt($("#MinKm").val());
let initialStartHrs = $("#StartHr").val();
let initialStartDate = $("#JourneyStartDate").val();
//let pickupDate = $("#pickupDate").val();

let txtStartKms = $("#StKm");
let txtEndKms = $("#EndKm");
let txtTotalHours = $("#TotalHrs");
var txtTotalKms = $("#TotalKm");
let txtMinHours = $("#MinHrs");
let txtStartDate = $("#JourneyStartDate");
let txtEndDate = $("#JourneyClosingDate")
let txtStartHour = $("#StartHr");
let txtEndHour = $("#EndHr");
let txtExtraHourRate = $("#ExtraHrsRate");
let txtExtraHourCharge = $("#ExtraHrsCharge");
let txtExtraHours = $("#ExtraHr");
let txtNightCharge = $("#NightCharge");
let txtRatePerNight = $("#NightRate");
let txtTollCharge = $("#TollCharge");
let txtParkingCharge = $("#ParkingCharge");
let txtInterstateCharges = $("#InterStateTax");
let txtMcd = $("#MCD");

$("#StKm,#EndKm").keyup(function () {
    calculateKms();
    calculateGt();
   
})
$('#StartHr').on('changeTime', function () {
    calculateHours();
});
$('#EndHr').on('changeTime', function () {
    calculateHours();
});

function calculateHours()
{
    
    let startKms = txtStartKms.val() ? parseInt(txtStartKms.val()) : 0;
    let endKms = txtEndKms.val() ? parseInt(txtEndKms.val()) : 0;
    let totalKms = endKms - startKms;

    let startDate = txtStartDate.val();
    let endDate = txtEndDate.val();
    let startHour = txtStartHour.val();
    let endHour = txtEndHour.val();
    if (endDate && startDate && endHour) {
        var data = {
            JourneyStartDate: startDate,
            JourneyEndDate: endDate,
            JourneyStartTime: startHour,
            JourneyEndTime: endHour
        };
        $.ajax({
            url: '/Booking/GetRemainingHour',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(data)
        }).done(function (r) {
           debugger
            let totalHrs =  parseFloat(r).toFixed(3);
            txtTotalHours.val(totalHrs);
            let minHours = parseInt(txtMinHours.val());
           
            if (totalHrs > minHours) {
                let extraTime = totalHrs - minHours;
                txtExtraHours.val(extraTime.toFixed(3));
                let extraTimeInMinutes = extraTime * 60;
                //rates
                let ratePerHour = parseFloat(txtExtraHourRate.val());
                let ratePerMin = ratePerHour / 60;
                let ratePerQuarterHour = ratePerMin*15;

                let extraCompleteHour = Math.floor(extraTimeInMinutes / 60);
                let remainMin = extraTimeInMinutes % 60;
                let quarterlyCharge = Math.ceil(remainMin / 15) * ratePerQuarterHour;

                let extraCharge = Math.round((extraCompleteHour * ratePerHour)+quarterlyCharge);
                txtExtraHourCharge.val(extraCharge);
            }
            calculateGt();
        })
            .fail(function (err) {
                console.log(err.responseText);
            });
    }
}

function calculateKms()
{
    let startKms = txtStartKms.val() ? parseInt(txtStartKms.val()) : 0;
    let endKms = txtEndKms.val() ? parseInt(txtEndKms.val()) : 0;
    let totalKms = endKms - startKms;
    txtTotalKms.val(totalKms);
    // hour textboxes
  
   
    /////

    txtStartDate.val(initialStartDate);
    txtEndDate.val("");
    txtStartHour.val(initialStartHrs);
    txtEndHour.val("00:00:00");
    txtTotalHours.val(0);
    txtExtraHours.val(0);
    txtExtraHourCharge.val(0);
}



$("#TollCharge,#ParkingCharge,#InterStateTax,#DiscountValue,#MCD").keyup(function () {
    calculateGt();
})


$("#TotalNight").keyup(function () {
    let txtTotalNight = $(this);
  
    let totalNights = txtTotalNight.val() ? parseInt(txtTotalNight.val()) : 0;
    let ratePerNight = txtRatePerNight.val() ? parseFloat(txtRatePerNight.val()) : 0;
    let totalCharge = Math.round(ratePerNight * totalNights);
    txtNightCharge.val(totalCharge);
    calculateGt();
})


function calculateGt() {
   
    let extraHourCharge = parseFloat(txtExtraHourCharge.val());
    let nightCharges = parseFloat(txtNightCharge.val());
    let tollCharge = txtTollCharge.val()?parseFloat(txtTollCharge.val()):0;
    let parkingCharge = txtParkingCharge.val() ? parseFloat(txtParkingCharge.val()) : 0;
    //let fuelCharge = parseFloat(txtFuelCharge.val());
    let interstateCharges = parseFloat(txtInterstateCharges.val());
    let mcd = parseFloat(txtMcd.val()?txtMcd.val():0);
    let finalCharge = Math.round(mcd + extraHourCharge + nightCharges + tollCharge+parkingCharge +interstateCharges);
    let txtNetAmount = $("#netAmount");
    txtNetAmount.val(finalCharge);
    let netAmt = parseFloat(finalCharge);
    let gt = netAmt;
   // let ddDiscountType = $("#DiscountType");
    //let txtDiscountValue = $("#DiscountValue");
    //let txtDiscountedAmount = $("#DiscountAmount");
    //if (ddDiscountType.val())
    //{
    //    var discountVal = parseFloat(txtDiscountValue.val());
    //    var discountAmt = 0;
    //    var dt = ddDiscountType.val();
    //    if(dt=="Flat")
    //    {
    //        discountAmt = discountVal;
    //    }
    //    else {
    //        discountAmt = Math.round((netAmt * discountVal) / 100,2);
    //    }
    //    if(discountAmt>0)
    //    {
    //        txtDiscountedAmount.val(discountAmt);
    //        let discountedAmt = netAmt - discountAmt;
    //        gt = discountedAmt;
    //    }
    //}
    
    $("#Total").val(gt);
}