#pragma checksum "D:\MyProject\PJW\PJW\Views\IntraUser\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d8e6b4b0ae8ee3ba9db846b272ccd747b5e6798d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_IntraUser_Index), @"mvc.1.0.view", @"/Views/IntraUser/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\MyProject\PJW\PJW\Views\_ViewImports.cshtml"
using PJW.Web;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d8e6b4b0ae8ee3ba9db846b272ccd747b5e6798d", @"/Views/IntraUser/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a1b9716aa90c7cef9b5e537ef4f2378438930e99", @"/Views/_ViewImports.cshtml")]
    public class Views_IntraUser_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\MyProject\PJW\PJW\Views\IntraUser\Index.cshtml"
  
    ViewBag.Title = "員工資訊列表";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"

<div id=""divIntraUserList"" class=""wrapper wrapper-content animated fadeInRight"">
    <div class=""ibox float-e-margins"">
        <div class=""ibox-content"">
            <div class=""row row-lg"">
                <div class=""col-sm-12"">
                    <div>
                        <div>
                            <div class=""btn-group hidden-xs"" id=""IntraUserTableEventsToolbar"" role=""group"">
                                <div class=""input-group"" style=""display: inline-block"">
                                    <input type=""text"" class=""form-control  input-md"" placeholder=""員工帳號"" id=""UserId"" style=""width: 150px;display: inline-block"" v-on:keyup.enter=""_FreshTable"" data-pure-clear-button>
                                </div>
                                <div class=""input-group"" style=""display: inline-block"">
                                    <input type=""text"" class=""form-control  input-md"" placeholder=""員工E-Mail"" id=""UserEmail"" style=""width: 150px;display: inline-block"" v-on:keyup.ente");
            WriteLiteral(@"r=""_FreshTable"" data-pure-clear-button>
                                </div>
                                <div class=""input-group"" style=""display: inline-block"">
                                    <input type=""text"" class=""form-control  input-md"" placeholder=""員工名稱"" id=""UserName"" style=""width: 150px;display: inline-block"" v-on:keyup.enter=""_FreshTable"" data-pure-clear-button>
                                </div>
                                <button class=""btn btn-w-m btn-primary authorization""
                                        action-id=""user-search""
                                        action-name=""條件查詢""
                                        style=""display: none;margin-right: 10px""
                                        v-on:click=""_FreshTable"">
                                    查詢
                                </button>
                            </div>
                            <table id=""IntraUserTable"" data-side-pagination=""server"" data-sort-order=""desc"" data-mob");
            WriteLiteral("ile-responsive=\"true\"></table>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">

        var vm = new Vue({
            el: '#divIntraUserList',
            data: {
            },
            computed: {},
            methods: {
                _FreshTable: function () {
                    try {
                        $("".bootstrap-table button[name='refresh']"")[0].click();
                    } catch (e) {

                    }
                }
            },
            mounted: function () {
                auth();//實現在auth.js內 是在_LayOut 裡面引入的
            }
        });

        $(function () {
            $('#IntraUserTable')
                .bootstrapTable({
                    ajax: ajaxRequest,
                    pageNumber: 1,
                    pageSize: 25,
                    pageList: [10, 25, 50, 100],
                    resetOffset: true,
                    search: false,
                    sortable: true,
                    pagination: true,
                    height: $(window).height(),
     ");
                WriteLiteral(@"               showRefresh: true,
                    showToggle: true,
                    showColumns: false,
                    striped: true,
                    sortOrder: 'asc',
                    clickToSelect: true,
                    singleSelect: true,
                    cache: false,
                    // showPaginationSwitch:true,
                    dataType: 'json',
                    iconSize: 'outline',
                    toolbar: '#IntraUserTableEventsToolbar',
                    icons: {
                        refresh: 'glyphicon-refresh',
                        toggleOff: 'glyphicon-list-alt icon-list-alt',
                        toggleOn: 'glyphicon-list-alt icon-list-alt',
                        columns: 'glyphicon-list'
                    },
                    columns: [

                        {
                            field: 'indx',
                            title: '編號',
                            sortable: true
                        },
");
                WriteLiteral(@"                        {
                            field: 'id',
                            title: '登入帳號',
                            sortable: true
                        },
                        {
                            field: 'name',
                            title: '姓名',
                            sortable: true
                        },
                        {
                            field: 'email',
                            title: 'E-Mail'
                        },
                        {
                            field: 'deptName',
                            title: '部門'
                        }
                    ],
                    onLoadSuccess: function () {
                        Tipped.create('.tip_infomation',
                            function (element) {
                                return ""<strong>"" + $(element).data('content') + ""</strong>"";
                            });
                    },
                    onToggle: fu");
                WriteLiteral(@"nction () {
                        tipReCreate();

                    }
                });

            $(window).resize(function () {
                tipReCreate();
                $('#IntraUserTable').bootstrapTable('resetView', { height: $(window).height() });
            });
        });

        function tipReCreate() {
            setTimeout(function () {
                Tipped.create('.tip_infomation',
                    function (element) {
                        return ""<strong>"" + $(element).data('content') + ""</strong>"";
                    });

            }, 200);

        }

        function ajaxRequest(params) {
            var pageSize = params.data.limit;
            var pageIndex = params.data.offset / params.data.limit + 1;
            var orderBy = params.data.sort;
            var orderSequence = params.data.order;

            QQT.ajax('/IntraUser/GetIntraUserList',
                'POST',
                {
                    pageIndex: pageIndex,
  ");
                WriteLiteral(@"                  pageSize: pageSize,
                    orderBy: orderBy,
                    orderSequence: orderSequence,
                    UserId: $.trim($('#UserId').val()),
                    UserEmail: $.trim($('#UserEmail').val()),
                    UserName: $.trim($('#UserName').val())
                })
                .done(function (response) {
                    params.success({
                        total: response.Total,
                        rows: response.Rows
                    });
                });

        }
    </script>

");
            }
            );
            WriteLiteral("\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
