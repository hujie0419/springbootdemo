<!doctype html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%= StatusCode >= 400 && StatusCode < 500 ? "服务器不能处理您的请求" : "服务器处理您的请求时出错" %></title>
    <%--<meta http-equiv="refresh" content="10;url=<%= DomainConfig.WwwSite %>" />--%>
    <link rel="shortcut icon" href="<%= DomainConfig.WwwSite %>/images/favicon.ico" type="image/x-icon" />
   <style type="text/css">
        body {
            margin: 0;
            padding: 0;
            background-color: white;
            /*background: url(<%= DomainConfig.ResourceSite %>/Image/Public/<%= StatusCode >= 400 && StatusCode < 500 ? "404" : "500" %>.jpg) center 132px no-repeat;*/
        }

        .logo {
            margin-top: 10px;
            display: block;
        }

        img {
            border: none;
        }

        a {
            text-decoration: none;
        }

        p {
            margin: 0;
            border: 0;
            padding: 0;
        }

        #ErrorPage500 {
            width: 990px;
            margin: auto;
            position: relative;
            height: 728px;
        }

            #ErrorPage500 div {
                position: absolute;
                left: 594px;
                top: 355px;
            }

                #ErrorPage500 div span {
                    color: white;
                    font-family: '微软雅黑','Times New Roman', Times, serif, Arial;
                    font-size: 12px;
                    text-align: right;
                    display: block;
                    width: 130px;
                    margin-bottom: 15px;
                }

                #ErrorPage500 div a {
                    color: #ffe400;
                    font-family: '微软雅黑','Times New Roman', Times, serif, Arial;
                    font-size: 18px;
                    font-weight: bold;
                    width: 176px;
                    height: 50px;
                    background-color: #005466;
                    display: block;
                    text-align: center;
                    line-height: 50px;
                    text-decoration: none;
                }

        #ErrorPage404 {
            width: 990px;
            margin: auto;
            position: relative;
            height: 728px;
        }

            #ErrorPage404 #Position404 {
                position: absolute;
                left: 573px;
                top: 132px;
            }

                #ErrorPage404 #Position404 .title {
                    font-size: 26px;
                    color: rgba(51, 51, 51, 1);
                    font-weight: bold;
                    margin: 30px 0;
                    font-family: '微软雅黑';
                }

            #ErrorPage404 .result {
                font-size: 14px;
                color: rgba(153, 153, 153, 1);
                font-family: '微软雅黑','Times New Roman', Times, serif, Arial;
            }

                #ErrorPage404 .result p {
                    line-height: 24px;
                }

                    #ErrorPage404 .result p:first-child {
                        margin-top: 10px;
                    }

                    #ErrorPage404 .result p span {
                        color: rgba(51, 51, 51, 1);
                    }

            #ErrorPage404 .other {
                margin-top: 30px;
                font-size: 14px;
                color: rgba(153, 153, 153, 1);
                font-family: '微软雅黑','Times New Roman', Times, serif, Arial;
            }

                #ErrorPage404 .other p {
                    line-height: 14px;
                    margin-top: 10px;
                }

                    #ErrorPage404 .other p a {
                        display: inline-block;
                        padding: 0 10px;
                        border-right: 1px solid rgba(189, 189, 189, 1);
                        color: rgba(4, 83, 156, 1);
                    }

                        #ErrorPage404 .other p a:first-child {
                            padding-left: 0;
                        }

                        #ErrorPage404 .other p a:last-child {
                            border-right: none;
                        }
    </style>
</head>
<body>
    <div id="ErrorPage500">
        <a href="<%= DomainConfig.WwwSite %>" title="途虎养车" class="logo">
            <img alt="途虎养车" src="<%= DomainConfig.ResourceSite %>/Image/Public/logo.png" /></a>
        <div id="Position">
            <span id="CountDown">10S后跳转到首页</span>
            <a href="<%= DomainConfig.WwwSite %>" title="途虎养车">返回首页 &gt;</a>
        </div>
    </div>
    <div id="ErrorPage404">
        <a href="<%= DomainConfig.WwwSite %>" title="途虎养车" class="logo">
            <img alt="途虎养车" src="<%= DomainConfig.ResourceSite %>/Image/Public/logo.png" /></a>
        <div id="Position404">
            <p class="title">亲，地球上没有您要访问的页面~</p>
            <div class="result">
                可能因为：
                <p><span>网址有错误></span>请检查地址是否完整或存在多余字符</p>
                <p><span>网址已失效></span>可能页面已删除，活动已下线等</p>
            </div>
            <div class="other">
                您可以换个姿势：
                 <p><a href="<%= DomainConfig.WwwSite %>">逛逛首页</a><a href="<%= DomainConfig.ProductSite %>/List/Tires.html">换换轮胎</a><a href="<%= DomainConfig.ProductSite %>/List/hub.html">看看轮毂</a><a href="<%= DomainConfig.BaoYangSite %>/baoyang/index.html">做做保养</a><a href="<%= DomainConfig.WwwSite %>/ChePin/index.aspx">买买车品</a></p>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        (function ()
        {var errorCode = <%=StatusCode%>;
            if (errorCode === 404) {
                document.getElementById("ErrorPage404").style.display = "block";
                document.getElementById("ErrorPage500").style.display = "none";
                document.getElementsByTagName("body")[0].setAttribute("style", "background:url(<%= DomainConfig.ResourceSite %>/Image/Public/404.png) center 132px no-repeat;");
            } else {
                document.getElementById("ErrorPage404").style.display = "none";
                document.getElementById("ErrorPage500").style.display = "block";
                document.getElementsByTagName("body")[0].setAttribute("style", "background:url(<%= DomainConfig.ResourceSite %>/Image/Public/500.jpg) center 132px no-repeat;");
                var timer = 10;
                var action = function() {
                    document.getElementById("CountDown").innerHTML = timer + "S后跳转到首页";
                    if (--timer < 0)
                        location.href = "<%= DomainConfig.WwwSite %>";
                    else
                        setTimeout(action, 1000);
                };
                action();

                var resize = function() {
                    document.getElementById("Position").style.left = 594 - (990 - Math.min(document.body.clientWidth, 990)) / 2 + "px";
                }
                window.onresize = resize;
                resize();
            }
        })();
    </script>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Tuhu.WebSite.Web.Activity.Error" %>

<!--<%= Context.Items["_ExceptionID_"] %>-->
