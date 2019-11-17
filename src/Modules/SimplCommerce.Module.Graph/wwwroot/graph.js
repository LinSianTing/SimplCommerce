var svg = d3.select("svg"),
    width = +svg.attr("width"),
    height = +svg.attr("height");

var color = d3.scaleOrdinal(d3.schemeCategory20);

var simulation = d3.forceSimulation()
    .force("link", d3.forceLink().id(function (d) { return d.id; }))
    .force("charge", d3.forceManyBody())
    .force("center", d3.forceCenter(width / 2, height / 2));

var HtmlUtil = {
    /*1.用浏览器内部转换器实现html转码*/
    htmlEncode: function (html) {
        //1.首先动态创建一个容器标签元素，如DIV
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerText(ie支持)或者textContent(火狐，google支持)
        (temp.textContent != undefined) ? (temp.textContent = html) : (temp.innerText = html);
        //3.最后返回这个元素的innerHTML，即得到经过HTML编码转换的字符串了
        var output = temp.innerHTML;
        temp = null;
        return output;
    },
    /*2.用浏览器内部转换器实现html解码*/
    htmlDecode: function (text) {
        //1.首先动态创建一个容器标签元素，如DIV
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerHTML(ie，火狐，google都支持)
        temp.innerHTML = text;
        //3.最后返回这个元素的innerText(ie支持)或者textContent(火狐，google支持)，即得到经过HTML解码的字符串了。
        var output = temp.innerText || temp.textContent;
        temp = null;
        return output;
    }
};

// Prepare data
var innerHTMLStr = document.getElementById('graphdata').innerHTML;

//alert(innerHTMLStr);

innerHTMLStr = HtmlUtil.htmlDecode(innerHTMLStr);

//alert(innerHTMLStr);
var graphData = JSON.parse(innerHTMLStr);

/*
 * Reference Doc : 
 * 1. https://ithelp.ithome.com.tw/articles/10197211
*/
function draw() {
    var data = graphData;

    var url_string = window.location.href;
    var url = new URL(url_string);
    var gn = url.searchParams.get("gn");

    if (gn != undefined) {
        var temData = data.nodes;
        var newData = [];

        $.each(temData, function (index, node) {
            if (node.group.indexOf(gn.substring(0,1)) != -1) {
                newData.push(node);
            }
        }); 

        data.nodes = newData;
    }

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

            if (d.newproducts > 0) {
                displaytext += " (" + d.newproducts + "本月新品 )";
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

            if (d.newproducts != 0) {
                window.open(
                    '/ui/' + d.id,
                    '_blank' // <- This is what makes it open in a new window.
                );
            }
            else {
                window.open(
                    'https://www.facebook.com/search/top/?q=' + d.name+'&epa=SEARCH_BOX' ,
                    '_blank' // <- This is what makes it open in a new window.
                );
            }
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
draw();
