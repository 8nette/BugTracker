google.charts.setOnLoadCallback(drawTaskOpenBugs);

function drawTaskOpenBugs() {
    $.ajax({
        url: '/Survey/GetJsonStatTaskOpenBugs',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            $.each(response, function (index, taskOpenBugs) {
                result.push(['' + taskOpenBugs.week + ', ' + taskOpenBugs.year,
                    taskOpenBugs.numberOfOpenBugs]);
            });

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Week');
            data.addColumn('number', 'Number of Bugs');
            data.addRows(result);

            var options = {
                title: 'Open Bugs under Closed Tasks',
                backgroundColor: { fill: 'transparent' },
                colors: ["#608f82"],
                legend: 'right',
                width: '1200',
                height: '300',
                chartArea: {
                    width: '900',
                    left: '100'
                }
            };

            var chart = new google.visualization.LineChart($("#taskOpenBugs")[0]);
            chart.draw(data, options);
        }
    });
}