<%@ Page Language="C#" %>
<%
    EFWCoreLib.CoreFrame.Business.SysLoginRight userSession = (EFWCoreLib.CoreFrame.Business.SysLoginRight)Session["RoleUser"];
%>
<%
  if (Session["RoleUser"] == null)
  {
%>
<script language="javascript">
    window.location.href = "/ModulePlugin/WebUIFrame/Aspx/metronic/login.html";
</script>
<%
  Response.End();
  }
%>

<!DOCTYPE html>

<!--[if IE 8]> <html lang="en" class="ie8 no-js"> <![endif]-->
<!--[if IE 9]> <html lang="en" class="ie9 no-js"> <![endif]-->
<!--[if !IE]><!-->
<html lang="en" class="no-js">
<!--<![endif]-->
<!-- BEGIN HEAD -->
<head>
<meta charset="utf-8"/>
<title>Hello,efwplus开发平台</title>
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta content="width=device-width, initial-scale=1" name="viewport"/>
<meta content="" name="description"/>
<meta content="" name="author"/>
<!-- BEGIN GLOBAL MANDATORY STYLES -->
<link href="/WebPlugin/metronic/assets/global/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic/assets/global/plugins/simple-line-icons/simple-line-icons.min.css" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic/assets/global/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic/assets/global/plugins/uniform/css/uniform.default.css" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css" rel="stylesheet" type="text/css"/>
<!-- END GLOBAL MANDATORY STYLES -->
<!-- BEGIN PAGE LEVEL PLUGIN STYLES -->
<link href="/WebPlugin/metronic/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.css" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic//assets/global/plugins/fancybox/source/jquery.fancybox.css" rel="stylesheet"/>
<!-- END PAGE LEVEL PLUGIN STYLES -->
<!-- BEGIN PAGE STYLES -->
<link href="/WebPlugin/metronic/assets/admin/pages/css/inbox.css" rel="stylesheet" type="text/css"/>
<!-- END PAGE STYLES -->
<!-- BEGIN THEME STYLES -->
<!-- DOC: To use 'rounded corners' style just load 'components-rounded.css' stylesheet instead of 'components.css' in the below style tag -->
<link href="/WebPlugin/metronic/assets/global/css/components.css" id="style_components" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic/assets/global/css/plugins.css" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic/assets/admin/layout/css/layout.css" rel="stylesheet" type="text/css"/>
<link href="/WebPlugin/metronic/assets/admin/layout/css/themes/darkblue.css" rel="stylesheet" type="text/css" id="style_color"/>
<link href="/WebPlugin/metronic/assets/admin/layout/css/custom.css" rel="stylesheet" type="text/css"/>
<!-- END THEME STYLES -->
<link rel="shortcut icon" href="favicon.ico"/>
<style>
    .page-content-wrapper {
        float: left;
        width: 100%;
        height:100%;
    }
    .page-content-wrapper .page-content {
        min-height:inherit;
        padding: 0 0 0 0px;
        height:100%;
    }
    .page-sidebar-fixed .page-sidebar {
        top: 0px; 
    }
    .portlet.box.grey-cascade > .portlet-title {
        background-color: #1caf9a;
    }
    .portlet.box.grey-cascade {
        border: 0px solid ;
    }
    .portlet.box > .portlet-body {
        background-color: #fff;
        padding: 5px;
        overflow:hidden;
    }
    .portlet > .portlet-title > .nav-tabs {
        background-color:#1caf9a;
        float: left;
    }
</style>
</head>
<!-- END HEAD -->
<!-- BEGIN BODY -->
<!-- DOC: Apply "page-header-fixed-mobile" and "page-footer-fixed-mobile" class to body element to force fixed header or footer in mobile devices -->
<!-- DOC: Apply "page-sidebar-closed" class to the body and "page-sidebar-menu-closed" class to the sidebar menu element to hide the sidebar by default -->
<!-- DOC: Apply "page-sidebar-hide" class to the body to make the sidebar completely hidden on toggle -->
<!-- DOC: Apply "page-sidebar-closed-hide-logo" class to the body element to make the logo hidden on sidebar toggle -->
<!-- DOC: Apply "page-sidebar-hide" class to body element to completely hide the sidebar on sidebar toggle -->
<!-- DOC: Apply "page-sidebar-fixed" class to have fixed sidebar -->
<!-- DOC: Apply "page-footer-fixed" class to the body element to have fixed footer -->
<!-- DOC: Apply "page-sidebar-reversed" class to put the sidebar on the right side -->
<!-- DOC: Apply "page-full-width" class to the body element to have full width page without the sidebar menu   
page-sidebar-fixed page-sidebar-closed-hide-logo-->
<body class="page-quick-sidebar-over-content page-sidebar-closed page-sidebar-closed-hide-logo">

<div class="clearfix">
</div>
<!-- BEGIN CONTAINER -->
<div class="page-container">
	<!-- BEGIN SIDEBAR -->
	<div class="page-sidebar-wrapper">
		<!-- DOC: Set data-auto-scroll="false" to disable the sidebar from auto scrolling/focusing -->
		<!-- DOC: Change data-auto-speed="200" to adjust the sub menu slide up/down speed -->
		<div class="page-sidebar navbar-collapse collapse">
			<!-- BEGIN SIDEBAR MENU -->
			<!-- DOC: Apply "page-sidebar-menu-light" class right after "page-sidebar-menu" to enable light sidebar menu style(without borders) -->
			<!-- DOC: Apply "page-sidebar-menu-hover-submenu" class right after "page-sidebar-menu" to enable hoverable(hover vs accordion) sub menu mode -->
			<!-- DOC: Apply "page-sidebar-menu-closed" class right after "page-sidebar-menu" to collapse("page-sidebar-closed" class must be applied to the body element) the sidebar sub menu mode -->
			<!-- DOC: Set data-auto-scroll="false" to disable the sidebar from auto scrolling/focusing -->
			<!-- DOC: Set data-keep-expand="true" to keep the submenues expanded -->
			<!-- DOC: Set data-auto-speed="200" to adjust the sub menu slide up/down speed -->
			<ul class="page-sidebar-menu page-sidebar-menu-closed" data-keep-expanded="false" data-auto-scroll="true" data-slide-speed="200">
				<li class="start active ">
					<a href="index.aspx">
					<i class="icon-home"></i>
					<span class="title">efwplus开发平台</span>
					</a>
                    
				</li>
                <li class="sidebar-toggler-wrapper" id="menuafter">
					<!-- BEGIN SIDEBAR TOGGLER BUTTON -->
					<div class="sidebar-toggler">
					</div>
					<!-- END SIDEBAR TOGGLER BUTTON -->
				</li>
				<!--动态生成菜单loadMenus()-->
			</ul>
			<!-- END SIDEBAR MENU -->
		</div>
	</div>
	<!-- END SIDEBAR -->
	<!-- BEGIN CONTENT -->
	<div class="page-content-wrapper">
		<div class="page-content">
            <div class="portlet grey-cascade box full-height-content full-height-content-scrollable">
                <div class="portlet-title">
							<ul class="nav nav-tabs" id="tab_menus">
								<li class="active" tabid="0">
									<a href="#portlet_tab_0" data-toggle="tab">首页</a>
								</li>
							</ul>

                           <div class="actions">
                                <a href="javascript:;" class="btn btn-default btn-sm" onclick="CloseMenu();"><i class="fa fa-times" ></i> 关闭 </a>
								<a href="javascript:;" class="btn btn-default btn-sm"><i class="icon-envelope-open"></i> 消息 </a>
								<div class="btn-group">
									<a class="btn btn-default btn-sm" href="javascript:;" data-toggle="dropdown">
									<i class="fa fa-user"></i> <%=userSession.EmpName%> <i class="fa fa-angle-down"></i>
									</a>
									<ul class="dropdown-menu pull-right">
										<li>
								            <a menuName="修改密码" MenuId="999" UrlName="/ModulePlugin/WebUIFrame/Aspx/SetPassWord.aspx" href="javascript:void();" onclick="OpenMenu(this);">
								            <i class="icon-lock"></i> 修改密码 </a>
							            </li>
							            <li>
								            <a href="login.html">
								            <i class="icon-key"></i> 退出 </a>
							            </li>
									</ul>
								</div>
							</div>

						</div>
						<div class="portlet-body">
                            <div class="tab-content" id="tab_contents">
								<div class="tab-pane active" id="portlet_tab_0">
                                   <div class="full-height-content full-height-content-scrollable">
								   <div class="full-height-content-body">
                                    <iframe  frameborder="no"  height="100%" width="100%" name="famRMain" src="/ModulePlugin/WebUIFrame/Aspx/task.aspx"></iframe>
                                   </div>
                                   </div>
								</div>
							</div>
                        </div>
			</div>
		</div>
	</div>
	<!-- END CONTENT -->
</div>
<!-- END CONTAINER -->
<!-- BEGIN FOOTER -->

<!-- END FOOTER -->
<!-- BEGIN JAVASCRIPTS(Load javascripts at bottom, this will reduce page load time) -->
<!-- BEGIN CORE PLUGINS -->
<!--[if lt IE 9]>
<script src="/WebPlugin/metronic/assets/global/plugins/respond.min.js"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/excanvas.min.js"></script> 
<![endif]-->
<script src="/WebPlugin/metronic/assets/global/plugins/jquery.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/jquery-migrate.min.js" type="text/javascript"></script>
<!-- IMPORTANT! Load jquery-ui.min.js before bootstrap.min.js to fix bootstrap tooltip conflict with jquery ui tooltip -->
<script src="/WebPlugin/metronic/assets/global/plugins/jquery-ui/jquery-ui.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/jquery.blockui.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/jquery.cokie.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/uniform/jquery.uniform.min.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js" type="text/javascript"></script>
<!-- END CORE PLUGINS -->
<!-- BEGIN PAGE LEVEL PLUGINS -->
<script src="/WebPlugin/metronic/assets/global/plugins/fancybox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/bootstrap-wysihtml5/wysihtml5-0.3.0.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.js" type="text/javascript"></script>

<!-- END PAGE LEVEL PLUGINS -->
<!-- BEGIN PAGE LEVEL SCRIPTS -->
<script src="/WebPlugin/metronic/assets/global/scripts/metronic.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/admin/layout/scripts/layout.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/admin/layout/scripts/quick-sidebar.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/admin/layout/scripts/demo.js" type="text/javascript"></script>
<script src="/WebPlugin/metronic/assets/admin/pages/scripts/inbox.js" type="text/javascript"></script>
<!-- END PAGE LEVEL SCRIPTS -->

<script src="/WebPlugin/JQueryCommon2.0.js" type="text/javascript"></script>
<script>
var _height=600;
jQuery(document).ready(function() {    
   Metronic.init(); // init metronic core componets
   Layout.init(); // init layout
   QuickSidebar.init(); // init quick sidebar
   //Demo.init(); // init demo features 
   //Index.init(); // init index page
    //Tasks.initDashboardWidget(); // init tash dashboard widget  
   loadMenus();
});
var m_icons = ['icon-users', 'icon-flag', 'icon-heart', 'icon-star', 'icon-envelope-open', 'icon-call-end', 'icon-pie-chart', 'icon-settings'];
function loadMenus() {
    requestAjax('Controller.aspx?controller=WebUIFrame@LoadingMenuController&method=GetLoginAllMenu', {}, function (data) {
        var moduledata = data;
        if (moduledata.length > 0) {
            var _html = '';
            for (var i = 0; i < moduledata.length; i++) {//模块
                _html += '<li><a href="javascript:;"><i class="'+m_icons[i]+'"></i><span class="title">' + moduledata[i].Name + '</span><span class="arrow "></span></a>';
                var menudata = moduledata[i].Menus;
                if (menudata.length > 0) {
                    _html += '<ul class="sub-menu">';
                    for (var k = 0; k < menudata.length; k++) {//一级菜单
                        if (menudata[k].FirstMenu.Name = '-1') {
                            var secondmenudata = menudata[k].SecondMenu;
                            if (secondmenudata.length > 0) {
                                for (var m = 0; m < secondmenudata.length; m++) {//二级菜单
                                    _html += '<li><a href="javascript:void();" menuName="' + secondmenudata[m].Name + '" MenuId="' + secondmenudata[m].MenuId + '" UrlName="/' + secondmenudata[m].UrlName + '" onclick="OpenMenu(this);"><i class="icon-doc"></i>' + secondmenudata[m].Name + '</a></li>';
                                }
                            }
                        } else {
                            _html += '<li><a href="javascript:;"><i class="icon-folder"></i>' + menudata[k].FirstMenu.Name + '<span class="arrow "></span></a>';
                            var secondmenudata = menudata[k].SecondMenu;
                            if (secondmenudata.length > 0) {
                                _html += '<ul class="sub-menu">';
                                for (var m = 0; m < secondmenudata.length; m++) {//二级菜单
                                    _html += '<li><a href="javascript:void();" menuName="' + secondmenudata[m].Name + '" MenuId="' + secondmenudata[m].MenuId + '" UrlName="/' + secondmenudata[m].UrlName + '" onclick="OpenMenu(this);"><i class="icon-doc"></i>' + secondmenudata[m].Name + '</a></li>';
                                }
                                _html += '</ul>';
                            }
                            _html += '</li>';
                        }
                    }
                    _html += '</ul>';
                }
                _html += '</li>';
            }
            $('#menuafter').after(_html);
        }

        _height=$(".page-content-wrapper").height() - 58;
    });
}

function OpenMenu(menu) {

    $('#tab_menus > li').removeClass("active");
    $('#tab_contents > div').removeClass("active");

    if ($('#tab_menus > li[tabid="' + $(menu).attr('MenuId') + '"]').length > 0) {
        $('#tab_menus > li[tabid="' + $(menu).attr('MenuId') + '"]').addClass("active");
        $('#tab_contents > div[id="portlet_tab_' + $(menu).attr('MenuId') + '"]').addClass("active");
    } else {
        var _htmlmenus = '';
        var _htmlcontents = '';
        _htmlmenus += '<li class="active" tabid=' + $(menu).attr('MenuId') + '><a href="#portlet_tab_' + $(menu).attr('MenuId') + '" data-toggle="tab">' + $(menu).attr('menuName') + '</a></li>';
        _htmlcontents += '<div class="tab-pane active" id="portlet_tab_' + $(menu).attr('MenuId') + '"><iframe id="iframepage_' + $(menu).attr('MenuId') + '" style="min-height:500px;" onLoad="reinitIframe(this)"  frameborder="no"  height="100%" width="100%" name="famRMain" src="' + $(menu).attr('UrlName') + '"></iframe></div>';
        $('#tab_menus').append(_htmlmenus);
        $('#tab_contents').append(_htmlcontents);
    }
}

function CloseMenu() {
    //alert($('#tab_menus').find('.active').length);
    if ($('#tab_menus').find('.active').length > 0) {
        if ($('#tab_menus').find('.active').attr("tabid") == "0") return;
        
        $('#tab_menus > li').remove('.active');
        $('#tab_contents > div').remove(".active");

        $('#tab_menus > li:last').addClass("active");
        $('#tab_contents > div:last').addClass("active");

    }
}

function reinitIframe(iframe) {
    //var iframe = document.getElementById("iframepage");
    try {
        //var bHeight = iframe.contentWindow.document.body.scrollHeight;
        //var dHeight = iframe.contentWindow.document.documentElement.scrollHeight;
        //var height = Math.max(bHeight, dHeight);
        iframe.height = _height;
    } catch (ex) { }
}



</script>
<!-- END JAVASCRIPTS -->
</body>
<!-- END BODY -->
</html>