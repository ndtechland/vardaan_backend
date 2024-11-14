
$("#addNewabt").click(function () {
   
    var noofbooking = $("#noBooking").val();
  
    var i;
    for (i = 0; i < noofbooking-1; i++) {

        let table = $("#attrTable");
        let lastRow = table.find("tr").last();
        let firstRow = table.find("tr").first();
        let firstInput = lastRow.find(":input").first();
        let nameOfFirstInput = firstInput.attr("name");
        let currentIndex = parseInt(nameOfFirstInput.replace(/[^\d.]/g, ''));
        let nextIndex = currentIndex + 1;
        let nextRow = lastRow.clone();

        window.aa = nextRow;

        nextRow.find('td').first().html(nextIndex + 1);
        nextRow.find(":input").each(function () {

            var currentInput = $(this);
            //alert(currentInput.prop('type'));
            if (currentInput.prop('type') == 'select-one') {
                var selectVal = currentInput.attr('sValue');
                //alert(selectVal);
                currentInput.val(selectVal);
            }

            // currentInput.val('');
            var name = currentInput.attr("name");
            var newName = name.replace(currentIndex, nextIndex);
            //alert(newName);
            currentInput.attr("name", newName);
        });
        nextRow.find("td").last().html('<span class="btn btn-danger" id="btnDeleteAttr"><i class="fa fa-trash-o"></i></span>');
        table.append(nextRow);
        $('.tp').timepicker({

            //defaultTime: '12:00:00',// 12:00:00 AM
            timeFormat: 'H:i:s',
            //minTime: '12:00:00', // 12:00:00 AM
            maxHour: 24,
            startTime: new Date(0, 0, 0, 12, 0, 0, 0), // 12:00:00 AM
            step: 05 // 05 minutes

        });
    }
});

// deleting attribue
$("#attrTable").on("click", "#btnDeleteAttr", function () {
    var btn = $(this);
    var currentRow = btn.parent().parent();
    currentRow.nextAll().each(function () {
        var row = $(this);
        var firstCol = row.find("td").first();
        var index = parseInt(firstCol.html());
        firstCol.html(index - 1);
    });
    // finding all rows after this row ad decrease their index by 1
    currentRow.nextAll().find(":input").each(function () {
        var row = $(this);
        var name = row.attr('name');
        var index = parseInt(name.replace(/[^\d.]/g, ''));
        var nextIndex = index - 1;
        var newName = name.replace(index, nextIndex);
        row.attr('name', newName);
    });
    currentRow.remove();
});