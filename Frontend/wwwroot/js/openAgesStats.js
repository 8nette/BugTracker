google.charts.setOnLoadCallback(drawOpenAges);

function drawOpenAges() {
    $.ajax({
        url: '/Survey/GetJsonStatOpenAges',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            $.each(response, function (index, openAge) {
                if (openAge.ageInWeeks == 1)
                    result.push([openAge.ageInWeeks + " week",
                    openAge.numberOfNotClosedBugs,
                        'color: white']);
                else if (openAge.ageInWeeks == 2)
                    result.push([openAge.ageInWeeks + " weeks",
                    openAge.numberOfNotClosedBugs,
                        'color: #e1ebe8']);
                else if (openAge.ageInWeeks == 3)
                    result.push([openAge.ageInWeeks + " weeks",
                    openAge.numberOfNotClosedBugs,
                        'color: #a7d1c5']);
                else if (openAge.ageInWeeks == 4)
                    result.push([openAge.ageInWeeks + " weeks",
                    openAge.numberOfNotClosedBugs,
                        'color: #89d6c0']);
                else if (openAge.ageInWeeks == 5)
                    result.push([openAge.ageInWeeks + " weeks",
                    openAge.numberOfNotClosedBugs,
                        'color: #748a83']);
                else if (openAge.ageInWeeks == 6)
                    result.push([openAge.ageInWeeks + " weeks",
                    openAge.numberOfNotClosedBugs,
                        'color: #5e8a7c']);
                else if (openAge.ageInWeeks == 7)
                    result.push([openAge.ageInWeeks + " weeks",
                    openAge.numberOfNotClosedBugs,
                        'color: #608f82']);
                else if (openAge.ageInWeeks == 8)
                    result.push([openAge.ageInWeeks + " weeks",
                    openAge.numberOfNotClosedBugs,
                        'color: #586e67']);
                else if (openAge.ageInWeeks == 9)
                    result.push(["more",
                        openAge.numberOfNotClosedBugs,
                        'color: #3a3d3b']);
            });

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Age');
            data.addColumn('number', 'Number of Bugs');
            data.addColumn({ role: 'style' });
            data.addRows(result);

            var view = new google.visualization.DataView(data);
            view.setColumns([0, 1,
                {
                    calc: "stringify",
                    sourceColumn: 1,
                    type: "string",
                    role: "annotation"
                },
                2]);

            var options = {
                title: 'Age of not Closed Bugs',
                backgroundColor: { fill: 'transparent' },
                colors: ['#89d6c0'],
                vAxis: {
                    title: 'Bugs'
                },
                height: '400',
                legend: {
                    position: 'none'
                }
            };

            var chart = new google.visualization.ColumnChart($("#openAges")[0]);
            chart.draw(view, options);
        }
    });
}