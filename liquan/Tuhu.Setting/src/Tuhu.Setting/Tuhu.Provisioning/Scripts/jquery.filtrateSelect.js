(function ()
{
	var filtrateSelect = function (target, option)
	{
		var self = $(target);

		if (!$.data(target, "__filtrateSelect__"))
		{
			var select = $('<div></div>');
			if (option.className)
				select.addClass(option.className);
			if (option.width === "auto")
				select.width(self.outerWidth());
			else
				select.width(option.width);

			var input = $('<input type="text" />');
			if (option.placeholder)
				input.attr("placeholder", option.placeholder);
			select.append(input);

			var ul = $("<ul></ul>");
			select.append(ul);
			if (option.maxHeight)
				ul.css("max-height", option.maxHeight);

			var blur = function (event)
			{
				//隐藏
				select.css("display", "");
				self.show();
			};

			var showInView = function (label)
			{
				label = $(label);
				var scrollTop = ul.scrollTop();
				var height = ul.height();
				var offset1 = ul.offset().top;
				var offset2 = label.offset().top;

				ul.scrollTop(scrollTop + offset2 - offset1 - height / 2);
				$("label", ul).removeClass("selected");
				label.addClass("selected");
			};

			var buildList = function (ul, options)
			{
				var fragment = document.createDocumentFragment();

				options.each(function (index)
				{
					var li = document.createElement("li");

					if (this.tagName === "OPTGROUP")
					{
						var div = document.createElement("div");
						div.innerHTML = this.label;
						li.appendChild(div);

						var u = document.createElement("ul");
						li.appendChild(u);

						buildList(u, $(this).children());
					} else
					{
						var label = document.createElement("label");
						label.innerHTML = this.text;
						if (this.selected)
							label.className = "selected";
						$.data(label, "option", this);
						$.data(this, "label", label);
						li.appendChild(label);
					}

					fragment.appendChild(li);
				});

				ul.appendChild(fragment);
			}

			self.focus(function ()
			{
				$("." + option.className).css("display", "").next().show();

				if (target.options.length > option.minOptions)
				{
					ul.html("");
					buildList(ul[0], self.children());

					select.width(self.outerWidth()).css("display", "inline-block");
					input.focus();
					self.css("display", "none");

					if (input.val())
					{
						input.trigger("input");
						$.each(target.options, function (index, option)
						{
							if (option.selected)
							{
								showInView($.data(option, "label"));
								return false;
							}
						});
					}

					$(document.body).one("click", blur);
				}
			});

			select.click(function (event)
			{
				event.stopPropagation();
			}) //.on("mousewheel", false);

			input.on("input", function (event)
			{
				var value = this.value;
				if (value)
					$("label", ul).removeClass("selected").each(function ()
					{
						var label = $(this);
						var text = label.text();

						var index = text.toLowerCase().indexOf(value.toLowerCase());
						if (index >= 0)
						{
							label.parent().show();
							label.html(text.substr(0, index) + "<span>" + text.substr(index, value.length) + "</span>" + text.substr(index + value.length));
						} else
							label.parent().hide();
					});
				else
					$("label", ul).removeClass("hover").each(function ()
					{
						var label = $(this);
						label.parent().show();
						label.text(label.text());
					});

				$("label:visible:first", ul).addClass("selected");
			}).keydown(function (event)
			{
				if (event.which == 13)
					return false;
			}).keyup(function (event)
			{
				var currentLi = $(".selected", ul);
				if (event.which == 13)
				{
					currentLi.trigger("click");
				} else if (ul.children().length > 1)
				{
					if (event.keyCode == 38 || event.key == "Up")
					{
						var visibles = $("label:visible", ul);
						var index = visibles.index(currentLi);
						if (index == 0)
							index = visibles.length;
						showInView(visibles[index - 1]);
					} else if (event.keyCode == 40 || event.key == "Down")
					{
						var visibles = $("label:visible", ul);
						var index = visibles.index(currentLi);
						if (index == visibles.length - 1)
							index = -1;
						showInView(visibles[index + 1]);
					}
				}
			});

			ul.on("click", "label", function ()
			{
				blur();

				var option = $.data(this, "option");
				if (!option.selected)
				{
					for (var index = 0, length = target.options.length; index < length; index++)
					{
						target.options[index].selected = false;
					}
					option.selected = true;
					self.trigger("change");
				}
			});

			self.before(select);
			$.data(target, "__filtrateSelect__", true);
		}
	}
	
	$.fn.filtrateSelect = function (selector, option)
	{
		///	<signature>
		///		<summary>下拉框筛选</summary>
		///		<returns type="this" />
		///	</signature>
		///	<signature>
		///		<summary>下拉框筛选</summary>
		///		<param name="selector" type="selector">选择器</param>
		///		<returns type="this" />
		///	</signature>
		///	<signature>
		///		<summary>下拉框筛选</summary>
		///		<param name="option" type="Object">选项</param>
		///		<returns type="this" />
		///	</signature>
		///	<signature>
		///		<summary>下拉框筛选</summary>
		///		<param name="selector" type="selector">选择器</param>
		///		<param name="option" type="Object">选项</param>
		///		<returns type="this" />
		///	</signature>
		if (typeof (selector) !== "string")
		{
			option = selector;
			selector = undefined;
		}
		option = $.extend({
			placeholder: "输入内容进行筛选",
			className: "filtrateSelect",
			width: "auto",
			minOptions: 8
		}, option);

		this.each(function ()
		{
			if (this.tagName === "SELECT")
				filtrateSelect(this, option);
			else
			{
				if (!selector)
					selector = "select";
				$(this).on("focusin", selector, function ()
				{
					if (this.tagName === "SELECT")
						filtrateSelect(this, option);
				});
			}
		});
		return this;
	}
})();