function PagerModel(pager)
{
	this.CurrentPage = pager.CurrentPage || 1;
	this.PageSize = pager.PageSize;
	this.TotalItem = pager.TotalItem;
	this.TotalPage = pager.TotalPage;
}

var RecordModel = {
	Pager: new PagerModel({}),
	VehicleRecord: [],
	Refresh: (function ()
	{
		var VehicleRecord = $('#VehicleRecord table tbody');
		var Pager = $('#pager');
		var TemplateVehicleRecordHtml = $('#TemplateVehicleRecordHtml').html();
		var TemplatePageHtml = $('#TemplatePageHtml').html();
		return function ()
		{
			if (RecordModel.VehicleRecord.length)
			{
				var html = TemplateEngine(TemplateVehicleRecordHtml, RecordModel.VehicleRecord);
				VehicleRecord.empty().append(html);
			}
			if (RecordModel.Pager.TotalPage)
			{
				var current = RecordModel.Pager.CurrentPage,
					total = RecordModel.Pager.TotalPage,
					start = 1,
					end = total;
				if (total > 20)
				{
					var nearStart = (current - 9) <= 1;
					var nearEnd = (current + 9) >= total;
					if (nearStart)
					{
						end = 20;
					} else if (nearEnd)
					{
						start = total - 19;
						end = total;
					} else
					{
						start = (current - 9);
						end = (current + 10);
					}
				}
				RecordModel.Pager.PageRange = [start, end];
				var html = TemplateEngine(TemplatePageHtml, RecordModel.Pager);
				Pager.empty().append(html);
			}
		}
	}())
};
var TemplateEngine = (function ()
{
	var TemplateHtml = '';
	var TemplateEngine = function (html, options)
	{
		html = html || TemplateHtml;
		var re = /<%([^%>]+)?%>/g,
			reExp = /(^( )?(if|for|else|switch|case|break|{|}))(.*)?/g,
			code = 'var r=[];\n',
			cursor = 0;
		var add = function (line, js)
		{
			js ? (code += line.match(reExp) ? line + '\n' : 'r.push(' + line + ');\n') :
				(code += line != '' ? 'r.push("' + line.replace(/"/g, '\\"') + '");\n' : '');
			return add;
		}
		while (match = re.exec(html))
		{
			add(html.slice(cursor, match.index))(match[1], true);
			cursor = match.index + match[0].length;
		}
		add(html.substr(cursor, html.length - cursor));
		code += 'return r.join("");';
		return new Function(code.replace(/[\r\t\n]/g, '')).apply(options);
	}
	return TemplateEngine;
}());
$(function ()
{
	//加载数据
	function GetVehicleRecord(ToPage)
	{
		$.post("/Record/VehicleStatistics", { CurrentPage: ToPage }, function (data)
		{
			if (data && data.Item1 && data.Item2)
			{
				RecordModel.Pager = new PagerModel(data.Item1);
				RecordModel.VehicleRecord = data.Item2;
				RecordModel.Refresh();
			}
		});
	}
	var colors = ['#F7464A', '#46BFBD', '#FDB45C', '#EF8730'];
	$.post("/Record/BrandStatistics", function (result)
	{
		var ctx = document.getElementById("myChart").getContext("2d");
		var data = [],other=1,PH=[["四虑","品牌","占比"]],html="";
		result.forEach(function (v, i)
		{
			data.push({ value: v.Percent, label: v.Brand, color: colors[i] });
			PH.push([v.CatalogName,v.Brand, v.Percent]);
			other -= v.Percent;
		});
		PH.forEach(function (v)
		{
			html+=  ["<tr><td>" ,v.join('</td><td>'),"</td></tr>" ].join("");
		});
		$('#BrandRecord table').append(html);
		//data.push({ value: other, label: '其他', color: '#fff' });
		//var myNewChart = new Chart(ctx).Doughnut(data);
	});
	var currentPage = Number(location.hash.replace('#', ''));
	GetVehicleRecord(currentPage ? currentPage : RecordModel.Pager.CurrentPage);
	$('#pager').on("click", "a", function ()
	{
		var self = $(this);
		if (self.hasClass("disabled")) return;
		var ToPage = self.attr("href").replace("#", "");
		if (ToPage)
		{
			GetVehicleRecord(ToPage);
		}
	});

});