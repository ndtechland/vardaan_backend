/// <reference path="jquery-1.12.4.js" />
$("#cityName").autocomplete({
    source: function (request, response) {
        $.ajax({
            url: '/Booking/GetCityList',
            data: { term: request.term },
            type: 'GET',
            success: function (data) {
                if (!data.length) {
                    var result = [
                     {
                         label: 'No data found',
                         value: response.term
                     }
                    ];
                    response(result);
                }
                else {
                    response($.map(data, function (item) {
                        return {
                            label: item.CityName + "(" + item.StateName + ")",
                            value: item.Id

                        };
                    }));
                }
            }
        });
    },
    select: function (event, ui) {
        $('#City_Id').val(ui.item.value);
        $('#cityName').val(ui.item.label);
        return false;
    }
});

$("#Client_Id").change(function () {
    var clientId = $(this).val();
    var packagetype = $("#PackageType_Id");
    packagetype.prop('selectedIndex', 0);
    
    var dd = $("#VehicleModel_Id");
    dd.empty();
    dd.append("<option value=''>Select Vehicle</option>");
    $.get('/Booking/VehicleForClient?clientId=' + clientId).then(function (r) {
        $.each(r, function (k, v) {
            dd.append("<option value=" + v.Id + ">" + v.ModelName + "</option>");
        });
    }).fail(function (r) {
        console.log(r.responseText)
    });
});

$("#BookingType").change(function () {
    var clientId = $("#Client_Id");

    //updated by bhupesh
    var dd = $("#VehicleModel_Id");
    dd.empty();
    dd.append("<option value=''>Select Vehicle</option>");

    //clientId.empty();
    //clientId.append("<option value=''>Select</option>");

    var ddNrgType = $("#NrgType");
    var regularDiv = $(".regular");
    var rgstate = $("#rgState");   
    var nrgDiv = $(".nrg");
    var clientId = $("#Client_Id");
    var paymentType = $("#NrdBookingMode");
    var nrdState = $("#NrdStateId");
    var bookingType = $(this).val();
    if (bookingType == "regular") {
        regularDiv.show();
        nrgDiv.hide();
        rgstate.show();
        paymentType.removeAttr("required");
        ddNrgType.removeAttr("required");
        nrdState.removeAttr("required");
        clientId.attr("required", "required");
        var custId = clientId.val();
        if (custId != null) {
            $.get('/Booking/VehicleForClient?clientId=' + custId).then(function (r) {
                $.each(r, function (k, v) {
                    dd.append("<option value=" + v.Id + ">" + v.ModelName + "</option>");
                });
            }).fail(function (r) {
                console.log(r.responseText)
            });
        }
    }
    else {

        paymentType.attr("required", "required");
        ddNrgType.attr("required", "required");
        nrdState.attr("required", "required");
        clientId.removeAttr("required");
        regularDiv.hide();
        nrgDiv.show();
        rgstate.hide();
    }
});

$("#NrgType").change(function () {
    var clientId = $("#Client_Id");
    var dd = $("#VehicleModel_Id");
    var packagetype = $("#PackageType_Id");
    var rentalType = $("#RentalType");
    var city_Id = $("#City_Id");
    var state = $("#NrdStateId");
    state.prop('selectedIndex',0);
    //state.append("<option value=''>Select State</option>");
    city_Id.empty();
    city_Id.append("<option value=''>Select City</option>");
    //rentalType.empty();
    //rentalType.append("<option value=''>Select </option>");
    packagetype.prop('selectedIndex',0);
    //packagetype.append("<option value=''>Select </option>");
    clientId.empty();
    clientId.append("<option value=''>Select Customer</option>");
    dd.empty();
    dd.append("<option value=''>Select Vehicle</option>");
    //clientId.empty();
    //clientId.append("<option value=''>Select</option>");
    var regularDiv = $(".regular");
    var nrgType = $(this).val();
    if (nrgType === "corporate")
    {
        regularDiv.show();
        clientId.attr("required", "required");
        var custId=clientId.val();
        if(custId!=null)
        {
            $.get('/Booking/VehicleForClient?clientId=' + custId).then(function (r) {
                $.each(r, function (k, v) {
                    dd.append("<option value=" + v.Id + ">" + v.ModelName + "</option>");
                });
            }).fail(function (r) {
                console.log(r.responseText)
            });
        }
    }
    else {
        clientId.removeAttr("required");
        regularDiv.hide();
       
        $.get('/Booking/VehicleForNrg').then(function (r) {
            $.each(r, function (k, v) {
                dd.append("<option value=" + v.Id + ">" + v.ModelName + "</option>");
            });
        }).fail(function (r) {
            console.log(r.responseText)
        });
    }
})

