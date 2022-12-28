google.charts.setOnLoadCallback(drawOverallPriority);

function drawOverallPriority() {
    $.ajax({
        url: '/Survey/GetJsonStatPriority',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            for (var i in response) {
                if (i == "numberOfCriticalBugs")
                    result.push(["Critical", response[i]]);
                if (i == "numberOfHighBugs")
                    result.push(["High", response[i]]);
                if (i == "numberOfNormalBugs")
                    result.push(["Normal", response[i]]);
                if (i == "numberOfLowBugs")
                    result.push(["Low", response[i]]);
            }

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Priority');
            data.addColumn('number', 'Quantity');
            data.addRows(result);

            var options = {
                title: 'Priority of all Bugs',
                is3D: false,
                colors: ["#89d6c0", "#608f82", "#7dab9e", "#748a83"],
                backgroundColor: { fill: 'transparent' },
                pieHole: '0.3',
                pieSliceTextStyle: {
                    color: "white"
                },
                legend: 'right',
                chartArea: {
                    width: '75%',
                    height: '10em'
                }
            };

            var chart = new google.visualization.PieChart($("#overallPriority")[0]);
            chart.draw(data, options);
        }
    });
}