#pragma checksum "D:\MyProject\PJW\PJW\Areas\Admin\Views\Role\RoleList.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f9f4072cd079edfbd04e3d9f0398fb6e159b8b91"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Role_RoleList), @"mvc.1.0.view", @"/Areas/Admin/Views/Role/RoleList.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f9f4072cd079edfbd04e3d9f0398fb6e159b8b91", @"/Areas/Admin/Views/Role/RoleList.cshtml")]
    public class Areas_Admin_Views_Role_RoleList : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/plugins/zTree/3.5/zTreeStyle/zTreeStyle.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-append-version", new global::Microsoft.AspNetCore.Html.HtmlString("true"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/plugins/zTree/3.5/jquery.ztree.all.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\MyProject\PJW\PJW\Areas\Admin\Views\Role\RoleList.cshtml"
  
    ViewBag.Title = "????????????";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "f9f4072cd079edfbd04e3d9f0398fb6e159b8b914287", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"


<div class=""wrapper wrapper-content animated fadeInRight"">
    <div class=""ibox float-e-margins"">

        <div class=""ibox-content"">
            <div class=""row row-lg"">
                <div class=""col-sm-12"">
                    <div>
                        <div>
                            <div class=""btn-group hidden-xs"" id=""RoleTableEventsToolbar"" role=""group"">
                                <button type=""button"" class=""btn btn-w-m btn-success authorization"" action-id=""role-add"" action-name=""????????????"" style=""display: none; margin-right: 10px"" v-on:click=""_roleAdd"">????????????</button>
                                <button type=""button"" class=""btn btn-w-m btn-success authorization"" action-id=""role-edit"" action-name=""????????????"" style=""display: none;margin-right: 10px"" v-on:click=""_roleEdit"">????????????</button>
                                <button type=""button"" class=""btn btn-w-m btn-primary authorization"" action-id=""role-copy"" action-name=""????????????"" style=""display: none;margin-right: 10px"" v-on:click=""_roleCopy"">????????????</but");
            WriteLiteral(@"ton>
                                <button type=""button"" class=""btn btn-w-m btn-danger authorization"" action-id=""role-delete"" action-name=""????????????"" style=""display: none;margin-right: 10px"" v-on:click=""_roleDelete"">????????????</button>
                                <div class=""input-group"" style=""display: inline-block"">
                                    <input type=""text"" placeholder=""?????????????????????"" class=""form-control input-md"" id=""roleName"" style=""width: 150px;display: inline-block"" v-on:keyup.enter=""_FreshTable"" data-pure-clear-button />
                                </div>
                                <div class=""input-group"" style=""display: inline-block"">
                                    <input type=""text"" placeholder=""?????????"" class=""form-control input-md"" id=""createUser"" style=""width: 150px;display: inline-block"" v-on:keyup.enter=""_FreshTable"" data-pure-clear-button />
                                </div>
                            </div>
                            <table id=""RoleTable"" data-side-paginati");
            WriteLiteral(@"on=""server"" data-sort-order=""desc"" data-mobile-responsive=""true""></table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>



<div class=""modal inmodal"" id=""myModal"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
    <div class=""modal-dialog"">
        <div class=""modal-content"">
            <div>
            </div>
            <div class=""modal-body"">
                <div class=""form-group"">
                    <label class=""control-label"">???????????????</label>
                    <input id=""firstname"" name=""firstname"" v-model=""roleAddName"" class=""form-control"" type=""text"" placeholder=""????????????????????????????????????"" data-pure-clear-button>
                </div>
                <div class=""form-group"">
                    <label class=""control-label"">???????????????</label>
                    <input id=""roleDesc"" name=""roleDesc"" v-model=""roleAddDesc"" class=""form-control"" type=""text"" placeholder=""?????????????????????"" data-pure-clear-button>
                </div>
        ");
            WriteLiteral(@"        <div class=""form-group"">
                    <label class=""control-label"">?????????????????????????????????????????????</label>
                    <ul id=""menu_tree"" class=""ztree"" style=""overflow: auto; ""></ul>
                </div>
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-white"" id=""close-model"" v-on:click=""_roleAddClose"">??????</button>
                <button type=""button"" class=""btn btn-primary"" id=""save-model"" v-on:click=""_roleAddSave"">??????</button>
            </div>
        </div>
    </div>
</div>

<div class=""modal inmodal"" id=""editModal"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
    <div class=""modal-dialog"">
        <div class=""modal-content"">
            <div>
            </div>
            <div class=""modal-body"">
                <div class=""form-group"">
                    <label class=""control-label"">???????????????</label>
                    <input v-model=""currentRow.RoleName"" class=""form-control"" type=""text"" placeholder=""????????????????????????????????????"">
               ");
            WriteLiteral(@" </div>
                <div class=""form-group"">
                    <label class=""control-label"">???????????????</label>
                    <input v-model=""currentRow.Description"" class=""form-control"" type=""text"" placeholder=""?????????????????????"">
                </div>
                <div class=""form-group"">
                    <label class=""control-label"">?????????????????????????????????????????????</label>
                    <ul id=""menu_tree_edit"" class=""ztree"" style=""overflow:auto;""></ul>
                </div>
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-white"" v-on:click=""_roleEditClose"">??????</button>
                <button type=""button"" class=""btn btn-primary"" v-on:click=""_roleEditSave"">??????</button>
            </div>
        </div>
    </div>
</div>
");
            DefineSection("Scripts", async() => {
                WriteLiteral("\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f9f4072cd079edfbd04e3d9f0398fb6e159b8b9110831", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"

    <script type=""text/javascript"">
        var zTree;
        var editTree;
        var vm = new Vue({
            el: 'body',
            data: {
                menuTree: [],
                roleAddName: '',
                roleAddDesc: '',
                currentRow: {}
            },
            computed: {},
            ready: function () {
                auth();
            },
            methods: {
                _FreshTable: function () {
                    try {
                        $("".bootstrap-table button[name='refresh']"")[0].click();
                    } catch (e) {

                    }
                },
                _roleCopy:function(){
                    var selectRow = $('#RoleTable').bootstrapTable('getSelections');
                    if (selectRow.length < 1) {
                        swal({
                            title: """",
                            text: ""??????????????????"",
                            type: ""error""
                        });
                        return");
                WriteLiteral(@";
                    }
                    var tid = selectRow[0].Tid;
                    QQT.ajax('/Admin/Home/GetMenuTree','POST',
                        {
                            roleId: tid
                        }
                    )
                    .done(function (response) {
                        if (editTree)editTree.destroy();
                        //??????Menu
                        var setting = {
                            check: {
                                enable: true
                            }
                        };

                        editTree = $.fn.zTree.init($(""#menu_tree_edit""), setting, response.Data);
                        $('#editModal').modal({ backdrop: 'static', keyboard: false });
                        $('#editModal').modal('show');
                    });

                },
                _roleDelete: function () {
                    var selectRow = $('#RoleTable').bootstrapTable('getSelections');
                    if (selectRow.length < 1");
                WriteLiteral(@") {
                        swal({
                            title: """",
                            text: ""??????????????????"",
                            type: ""error""
                        });
                        return;
                    }

                    var tid = selectRow[0].Tid;
                    swal({
                        title: ""??????????????????????"",
                        text: ""?????????????????????????????????????????????"",
                        type: ""warning"",
                        showCancelButton: true,
                        confirmButtonColor: ""#DD6B55"",
                        confirmButtonText: ""???????????????????????????"",
                        cancelButtonText: ""????????????????????????"",
                        showLoaderOnConfirm: true,
                        preConfirm: function (isConfirm) {
                            if (isConfirm) {
                                QQT.ajax('/Admin/Role/DeleteRole',
                                    'Post',
                                    { tid: tid })
                                    .done(fu");
                WriteLiteral(@"nction (response) {
                                        $('#RoleTable').bootstrapTable('refresh', { silent: true });
                                        swal(""????????????"", ""????????????!"", ""success"");
                                    });
                            }
                        }
                    }).then(function () {

                    }).catch(function () {

                    });
                },
                _roleAdd: function () {
                    vm.roleAddName = '';
                    vm.roleAddDesc = '';
                    $('#myModal').modal({ backdrop: 'static', keyboard: false });
                    $('#myModal').modal('show');
                },
                _roleAddClose: function () {
                    resetTree(zTree);
                    $('#myModal').modal('hide');
                },
                _roleAddSave: function () {
                    if ($.trim(vm.roleAddName).length < 1) {
                        swal({
                            title: """",
     ");
                WriteLiteral(@"                       text: ""????????????????????????!"",
                            type: ""error""
                        });
                        return;
                    }
                    var ac = getNodeIdAction(zTree);
                    QQT.ajax('/Admin/Role/AddRole',
                        'POST',
                        {
                            RoleName: vm.roleAddName,
                            RoleDesc: vm.roleAddDesc,
                            Ids: ac.ids,
                            Actions: ac.actions
                        },
                        true)
                        .done(function (response) {
                            $('#myModal').modal('hide');
                            resetTree(zTree);
                            $('#RoleTable').bootstrapTable('refresh', { silent: true });
                        });

                },
                _roleEdit: function () {
                    var selectRow = $('#RoleTable').bootstrapTable('getSelections');
                    if ");
                WriteLiteral(@"(selectRow.length < 1) {
                        swal({
                            title: """",
                            text: ""??????????????????"",
                            type: ""error""
                        });
                        return;
                    }
                    
                    vm.currentRow = $.extend({}, selectRow[0]);

                    QQT.ajax('/Admin/Home/GetMenuTree',
                        'POST',
                        {
                            roleId: vm.currentRow.Tid
                        })
                        .done(function (response) {
                            if (editTree)editTree.destroy();
                            //??????Menu
                            var setting = {
                                check: {
                                    enable: true
                                }
                            };

                            editTree = $.fn.zTree.init($(""#menu_tree_edit""), setting, response.Data);
                            ");
                WriteLiteral(@"$('#editModal').modal({ backdrop: 'static', keyboard: false });
                            $('#editModal').modal('show');
                        });


                },
                _roleEditClose: function () {
                    $('#editModal').modal('hide');
                },
                _roleEditSave: function () {
                    if ($.trim(vm.currentRow.RoleName).length < 1) {
                        swal({
                            title: """",
                            text: ""????????????????????????!"",
                            type: ""error""
                        });
                        return;
                    }
                    var ac = getNodeIdAction(editTree);
                    QQT.ajax('/Admin/Role/AddRole',
                        'POST',
                        {
                            RoleName: vm.currentRow.RoleName,
                            RoleDesc: vm.currentRow.Description,
                            Ids: ac.ids,
                            Actions: ac.actions");
                WriteLiteral(@",
                            Tid: vm.currentRow.Tid
                        },
                        true)
                        .done(function (response) {
                            $('#editModal').modal('hide');
                            resetTree(zTree);
                            vm.currentRow.RoleName = '';
                            vm.currentRow.Description = '';
                            $('#RoleTable').bootstrapTable('refresh', { silent: true });
                            swal(""????????????"", ""????????????!"", ""success"");
                        });

                }
            }
        });

        $(function () {
            $('#RoleTable')
                .bootstrapTable({
                    ajax: ajaxRequest,
                    pageNumber: 1,
                    pageSize: 10,
                    pageList: [10, 25, 50, 100],
                    resetOffset: true,
                    search: false,
                    sortable: true,
                    pagination: true,
                    heig");
                WriteLiteral(@"ht: $(window).height(),
                    showRefresh: true,
                    showToggle: true,
                    showColumns: false,
                    striped: true,
                    sortOrder: 'desc',
                    clickToSelect: true,
                    singleSelect: true,
                    cache: false,
                    // showPaginationSwitch:true,
                    dataType: ""json"",
                    iconSize: 'outline',
                    toolbar: '#RoleTableEventsToolbar',
                    icons: {
                        refresh: 'glyphicon-refresh',
                        toggleOff: 'glyphicon-list-alt icon-list-alt',
                        toggleOn: 'glyphicon-list-alt icon-list-alt',
                        columns: 'glyphicon-list'
                    },
                    columns: [
                        {
                            field: 'state',
                            title: '',
                            checkbox: true
                        },
  ");
                WriteLiteral(@"                      {
                            field: 'Tid',
                            title: '??????Id'
                        },
                        {
                            field: 'RoleName',
                            title: '????????????'
                        },
                        {
                            field: 'Description',
                            title: '????????????'
                        },
                        {
                            field: 'CreateUser',
                            title: '?????????'
                        }
                    ],
                    onLoadSuccess: function () {

                    },
                    onToggle: function () {
                    },
                    onEditableSave: function (field, row, oldValue, $el) {

                    }
                });

            $(window).resize(function () {
                $('#RoleTable').bootstrapTable('resetView', { height: $(window).height() });
            });

            //???????????????menu
 ");
                WriteLiteral(@"           QQT.ajax('/Admin/Home/GetMenuTree', 'Post')
                .done(function (response) {
                    console.log(response);
                    vm.menuTree = response.Data;
                    var setting = {
                        check: {
                            enable: true
                        }
                    };
                    zTree = $.fn.zTree.init($(""#menu_tree""), setting, response.Data);
                });
        });


        function ajaxRequest(params) {
            var pageSize = params.data.limit;
            var pageIndex = params.data.offset / params.data.limit + 1;
            var orderBy = params.data.sort;
            var orderSequence = params.data.order;
            QQT.ajax('/Admin/Role/GetRoleList',
                'POST',
                {
                    pageIndex: pageIndex,
                    pageSize: pageSize,
                    orderBy: orderBy,
                    orderSequence: orderSequence,
                    roleName: $.trim($('#r");
                WriteLiteral(@"oleName').val()),
                    createUser: $.trim($('#createUser').val())
                })
                .done(function (response) {
                    params.success({
                        total: response.Total,
                        rows: response.Rows
                    });
                });

        }

        function getNodeIdAction(tree) {
            var nodes = tree.getCheckedNodes();
            var tmpNode;
            var result = {};
            var ids = [];
            var actions = [];
            for (var i = 0; i < nodes.length; i++) {
                tmpNode = nodes[i];
                if (tmpNode.Tid !== 0) {
                    ids.push(tmpNode.Tid);
                } else {
                    actions.push({
                        MenuId: tmpNode.ParentTid,
                        ActionId: tmpNode.ActionId,
                        ActionName: tmpNode.name
                    });
                }
            }
            result.ids = ids;
            result.actions");
                WriteLiteral(" = actions;\n            return result;\n        }\n\n        function resetTree(tree) {\n            if (!tree) {\n                return;\n            }\n            tree.checkAllNodes(false);\n            tree.expandAll(false);\n        }\n\n    </script>\n\n");
            }
            );
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
