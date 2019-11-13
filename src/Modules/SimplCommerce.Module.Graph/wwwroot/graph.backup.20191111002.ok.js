
var w = $("#divforgraph").width();
var h = $("#divforgraph").height();

if (h < 400) {
    h = 600;
}
var color = d3.scaleOrdinal(d3.schemeCategory20);

var cola = cola.d3adaptor(d3).size([w, h]);

var svg = d3.select("#divforgraph").append("svg")
    .attr("width", w)
    .attr("height", h);

d3.json("modules/graph/data.json", function (error, graph) {

    cola
        .nodes(graph.nodes)
        .links(graph.links)
        .jaccardLinkLengths(40, 0.7)
        .start(30);

    var link = svg.selectAll(".link")
        .data(graph.links)
        .enter().append("line")
        .attr("class", "link")
        .style("stroke-width", function (d) { return Math.sqrt(d.value); });

    var node = svg.selectAll(".node")
        .data(graph.nodes)
        .enter().append("circle")
        .attr("class", "node")
        .attr("r", 5)
        .style("fill", function (d) { return color(d.group); })
        .call(cola.drag);

    node.append("title").text(function (d) { return d.name; });

    cola.on("tick", function () {
        link.attr("x1", function (d) { return d.source.x; })
            .attr("y1", function (d) { return d.source.y; })
            .attr("x2", function (d) { return d.target.x; })
            .attr("y2", function (d) { return d.target.y; });

        node.attr("cx", function (d) { return d.x; }).attr("cy", function (d) { return d.y; });
    });

    $("#nodeInformation").hide();
    $(".node").click(function () {
        $("#nodeInformation").show();
    });
    $("#nodeInformation").mouseout(function () {
        $("#nodeInformation").hide();
    });

});

