google.charts.setOnLoadCallback(drawDeveloperProductivity);

function drawDeveloperProductivity() {
    $.ajax({
        url: '/Survey/GetJsonDeveloperProductivityStats',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            $.each(response, function (index, developerStats) {
                result = [];
                result.push([
                    "Open", developerStats.numberOfOpenBugs
                ]);
                result.push([
                    "Cannot Reproduce", developerStats.numberOfNotReproduceBugs
                ]);
                result.push([
                    "Will not Fix", developerStats.numberOfNotFixBugs
                ]);
                result.push([
                    "Ready for Testing", developerStats.numberOfTestBugs
                ]);
                result.push([
                    "Resolved", developerStats.numberOfResolvedBugs
                ]);

                var data = new google.visualization.DataTable();
                data.addColumn('string', 'Resolution');
                data.addColumn('number', 'Quantity');
                data.addRows(result);

                var options = {
                    title: 'Resolution of Bugs for Top 4 Productive Developers: ' + developerStats.developerUsername,
                    is3D: false,
                    colors: ["#89d6c0", "#748a83", "#3a3d3b", "#7dab9e", "#608f82", "#ffffff"],
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

                var chart = new google.visualization.PieChart($("#developerProductivity" + index)[0]);
                chart.draw(data, options);
            });
        }
    });
}