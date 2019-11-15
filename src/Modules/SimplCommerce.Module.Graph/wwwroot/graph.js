var svg = d3.select("svg"),
    width = +svg.attr("width"),
    height = +svg.attr("height");

var color = d3.scaleOrdinal(d3.schemeCategory20);

var simulation = d3.forceSimulation()
    .force("link", d3.forceLink().id(function (d) { return d.id; }))
    .force("charge", d3.forceManyBody())
    .force("center", d3.forceCenter(width / 2, height / 2));

// Prepare data
var data = JSON.parse(document.getElementById('graphdata').innerHTML);

/*
 * Reference Doc : 
 * 1. https://ithelp.ithome.com.tw/articles/10197211
*/
function draw(data) {

    var link = svg.append("g")
        .attr("class", "links")
        .selectAll("line")
        .data(data.links)
        .enter().append("line")
        .attr("stroke-width", function (d) { return Math.sqrt(d.value); });

    var node = svg.append("g")
        .attr("class", "nodes")
        .selectAll("g")
        .data(data.nodes)
        .enter().append("g")

    var circles = node.append("circle")
        .attr("r", function (d) { return d.point ? d.point : 5 })
        .attr("fill", function (d) { return color(d.color); })
        .call(d3.drag()
            .on("start", dragstarted)
            .on("drag", dragged)
            .on("end", dragended));

    var lables = node.append("text")
        .text(function (d) {

            var displaytext = d.name

            if (d.group != "") {
                displaytext += "-" + d.group;
            }

            if (d.buy > 0) {
                displaytext += " 買" + d.buy + "次";
            }

            if (d.sell > 0) {
                displaytext += " 賣" + d.sell + "次";
            }

            return displaytext;
        })
        .attr('x', 6)
        .attr('y', 3)
        .on("click", function (d) {
            //alert(d.name)
            window.open(
                '/un/' + d.name,
                '_blank' // <- This is what makes it open in a new window.
            );
        });

    node.append("title")
        .text(function (d) { return d.name; });

    simulation
        .nodes(data.nodes)
        .on("tick", ticked);

    simulation.force("link")
        .links(data.links)
        .distance(90);

    function ticked() {
        link
            .attr("x1", function (d) { return d.source.x; })
            .attr("y1", function (d) { return d.source.y; })
            .attr("x2", function (d) { return d.target.x; })
            .attr("y2", function (d) { return d.target.y; });

        node
            .attr("transform", function (d) {
                return "translate(" + d.x + "," + d.y + ")";
            })
    }

}

function dragstarted(d) {
    if (!d3.event.active) simulation.alphaTarget(0.3).restart();
    d.fx = d.x;
    d.fy = d.y;
}

function dragged(d) {
    d.fx = d3.event.x;
    d.fy = d3.event.y;
}

function dragended(d) {
    if (!d3.event.active) simulation.alphaTarget(0);
    d.fx = null;
    d.fy = null;
}

// draw
draw(data);
