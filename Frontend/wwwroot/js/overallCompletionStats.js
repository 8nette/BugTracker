google.charts.setOnLoadCallback(drawOverallCompletion);

function drawOverallCompletion() {
    $.ajax({
        url: '/Survey/GetJsonStatCompletion',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            for (var i in response) {
                if (i == "numberOfOpenBugs")
                    result.push(["Open", response[i]]);
                if (i == "numberOfNotReproduceBugs")
                    result.push(["Cannot Reproduce", response[i]]);
                if (i == "numberOfNotFixBugs")
                    result.push(["Will not Fix", response[i]]);
                if (i == "numberOfTestBugs")
                    result.push(["Ready for Testing", response[i]]);
                if (i == "numberOfResolvedBugs")
                    result.push(["Resolved", response[i]]);
            }

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Resolution');
            data.addColumn('number', 'Quantity');
            data.addRows(result);

            var options = {
                title: 'Resolution of all not Closed Bugs',
                is3D: false,
                colors: ["#89d6c0", "#748a83", "#3a3d3b", "#7dab9e", "#608f82"],
                backgroundColor: { fill: 'transparent' },
                pieHole: '0.3',
                pieSliceTextStyle: {
                    color: "white"
                },
                legend: 'left',
                chartArea: {
                    width: '75%',
                    height: '10em'
                }
            };

            var chart = new google.visualization.PieChart($("#overallCompletion")[0]);
            chart.draw(data, options);
        }
    });
}
