#pragma checksum "D:\MyProject\PJW\PJW\Areas\Admin\Views\Role\RoleAction.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9ba5f51fd119e6ac9d9cd0760e1ddd9c15e1bf7b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Role_RoleAction), @"mvc.1.0.view", @"/Areas/Admin/Views/Role/RoleAction.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9ba5f51fd119e6ac9d9cd0760e1ddd9c15e1bf7b", @"/Areas/Admin/Views/Role/RoleAction.cshtml")]
    public class Areas_Admin_Views_Role_RoleAction : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/plugins/zTree/3.5/metroStyle/metroStyle.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-append-version", new global::Microsoft.AspNetCore.Html.HtmlString("true"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/plugins/zTree/3.5/jquery.ztree.all.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 1 "D:\MyProject\PJW\PJW\Areas\Admin\Views\Role\RoleAction.cshtml"
  
    ViewBag.Title = "許可權管理";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "9ba5f51fd119e6ac9d9cd0760e1ddd9c15e1bf7b4304", async() => {
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


<style>
    body{
        min-width: 800px;
        overflow-x: auto;
    }

    div {
        display: inline;
    }

        div.left {
            float: left;
            width: 250px;
            height: 80%;
            overflow: auto;
        }

        div.right {
            float: left;
            width: 500px;
            height: 100%;
            overflow: auto;
        }

        div.middle {
            float: left;
            width: 100px;
            height: 80%;
            overflow: auto;
            padding-top: 250px;
            text-align: center;
        }
        
        .ztree li span.button.chk.checkbox_true_full {
            background-position: -26px -5px;
            background-color: #05ef834d;
        }
</style>

<div class=""left"">
    <ul id=""tree"" class=""ztree""></ul>
</div>

<div class=""middle"">
    <button type=""button"" class=""btn btn-success authorization"" style=""display: none"" action-id=""role-action-save"" v-on:click=""RoleActionSave"" action-name=""提交儲存"">
        提交儲存
 ");
            WriteLiteral(@"   </button>
</div>

<div class=""right"" style=""display: none"" v-show=""showControl"">
    <div class=""searchAreaBox"">
    	<div class=""searchArea"">
    		<input type=""text"" name=""searchArea"" placeholder=""輸入搜索關鍵字"">
    		<i class=""searchAreaBtn fa fa-search""></i>
    	</div>
    	<div class=""clearft""></div>
    </div>	
    <ul id=""tree2"" class=""ztree""></ul>
</div>

");
            DefineSection("Scripts", async() => {
                WriteLiteral("\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9ba5f51fd119e6ac9d9cd0760e1ddd9c15e1bf7b7059", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
    <script type=""text/javascript"">

        var vm = new Vue({
            el: 'body',
            data: {
                showControl: false,
                currentRow: {}
            },
            computed: {},
            methods: {
                RoleActionSave: function () {
                    var tree1Obj = $.fn.zTree.getZTreeObj(""tree"");
                    var nodes1 = tree1Obj.getSelectedNodes();
                    if (nodes1.length !== 1) {
                        swal({
                            title: ""提交失敗！"",
                            text: '沒有要修改的!',
                            type: ""error""
                        });
                        return;
                    }
                    var node = nodes1[0].getParentNode();
                    if (node == null) {
                        swal({
                            title: ""提交失敗！"",
                            text: '獲取數據失敗!',
                            type: ""error""
                        });
                        return");
                WriteLiteral(@";
                    }
                    var selectNodes = getSelectedNodes();
                    QQT.ajax('/Admin/Role/AddRoleActions',
                        'POST',
                        {
                            ActionId: nodes1[0].ActionId,
                            MenuId: node.Tid,
                            ActionList: selectNodes
                        },
                        true)
                        .done(function (response) {
                            swal(""成功啦！"", ""操作成功!"", ""success"");
                        });
                }
            },
            ready: function () {
                auth();
                
                 $(window).resize(function () {
                     //設定tree的初始化高度
                    $('.left').css('height',$(window).height());
                    $('.right').css('height',$(window).height());
                    $('.right').css('width',$(window).width()-250-100);
                 });
                 
                $('.left').css('heigh");
                WriteLiteral(@"t',$(window).height());
                $('.right').css('height',$(window).height());
                $('.right').css('width',$(window).width()-250-100);
                //獲取總的數據
                QQT.ajax('/Admin/Home/GetMenuActionTree', 'Post', {})
                    .done(function (response) {
                        ready(response.Data);
                    });

                QQT.ajax('/Admin/Home/GetControllerActions', 'Post', {})
                    .done(function (response) {
                        ready2(response.Data);
                    });
            }
        });

        function ready(zNodes) {
            var setting = {
                view: {
                    selectedMulti: false
                },
                check: {
                    enable: false
                },
                callback: {
                    onClick: onClick
                },
                 usericon:{
                     // 摺疊icon
                     OPEN: ""open fa ftopen fa-folder-open"", // 打開檔案圖示
 ");
                WriteLiteral(@"                    CLOSE: ""close fa ftclose fa-folder"",   // 摺疊檔案圖示
                     OPENDK:'open icon-folder ace-icon tree-minus',  // 打開狀態圖示
                     CLOSEZD:'close icon-folder ace-icon tree-plus', // 摺疊狀態圖示
                     DOCU: ""docu"",
                     // 子節點圖示
                     CHILDRENNODE: ""fa-book"",
                     // 覈取方塊圖示
                     UNFACHECK: ""fa fa-check"",
                     FACHECKED: ""fa fa-times"",
                     // 編輯圖示
                     EDIT: ""fa-pencil-square-o"",
                     // 移除圖示
                     REMOVE: ""fa-eraser"",
                     // 增加節點圖示
                     ADDNODE: ""fa-plus-square""
                 }
                
            };

            $.fn.zTree.init($(""#tree""), setting, zNodes);
            var treeObj = $.fn.zTree.getZTreeObj(""tree"");
            treeObj.expandAll(true);
        }

        function ready2(zNodes) {
            var setting = {
                view: {
                    selectedMult");
                WriteLiteral(@"i: true
                },
                check: {
                    enable: true
                },
              usericon:{
                  // 摺疊icon
                  OPEN: ""open fa ftopen fa-folder-open"", // 打開檔案圖示
                  CLOSE: ""close fa ftclose fa-folder"",   // 摺疊檔案圖示
                  OPENDK:'open icon-folder ace-icon tree-minus',  // 打開狀態圖示
                  CLOSEZD:'close icon-folder ace-icon tree-plus', // 摺疊狀態圖示
                  DOCU: ""docu"",
                  // 子節點圖示
                  CHILDRENNODE: ""fa-book"",
                  // 覈取方塊圖示
                  UNFACHECK: ""fa fa-check"",
                  FACHECKED: ""fa fa-times"",
                  // 編輯圖示
                  EDIT: ""fa-pencil-square-o"",
                  // 移除圖示
                  REMOVE: ""fa-eraser"",
                  // 增加節點圖示
                  ADDNODE: ""fa-plus-square""
              }
            };

            $.fn.zTree.init($(""#tree2""), setting, zNodes);
            var treeObj = $.fn.zTree.getZTreeObj(""tree2"");
    ");
                WriteLiteral(@"        treeObj.expandAll(true);

        }

        function onClick(event, treeId, treeNode, clickFlag) {
            if (treeNode.Tid !== 0) {
                vm.showControl = false;
                return;
            }
            QQT.ajax('/Admin/Role/GetRoleActions',
                'POST',
                {
                    ActionId: treeNode.ActionId,
                    MenuId: treeNode.getParentNode().Tid
                })
                .done(function (response) {
                    $.fn.zTree.destroy(""tree2"");
                    ready2(response.Data);
                    vm.showControl = true;
                });
        }

        function getSelectedNodes() {
            var treeObj = $.fn.zTree.getZTreeObj(""tree2"");
            var nodes = treeObj.getCheckedNodes(true);
            var re = [];
            for (var i = 0; i < nodes.length; i++) {
                var node = nodes[i];
                if (node.children != null) continue;
                re.push(node.getParentNode().Name + ");
                WriteLiteral(@"',' + node.Name);
            }
            return re;
        }

        
    // 地區搜索
    var parentArray = [];
    var childArray = [];

    // 遞迴獲取目標節點所有父節點
    function getParentsNode(treeNode){
        var thisParentNode = treeNode.getParentNode(); //得到該節點的父節點
        if( thisParentNode != null ){ // 父節點存在
            parentArray.push(thisParentNode); // 儲存至陣列
            getParentsNode(thisParentNode); // 重調 
        }else{
            return false;
        }           
    }
    // 遞迴獲取目標節點所有子節點
    function getChildrenNode(treeNode){
        var thisIsParent = treeNode.isParent; // 獲取目標節點 isParent 屬性，判斷是否為父節點
        if( thisIsParent == true ){
            var thisChildrenNode = treeNode.children; // 得到該節點的子節點集合
            for(var i=0;i<thisChildrenNode.length;i++){
                childArray.push(thisChildrenNode[i]); // 將該子節點加入陣列中
                getChildrenNode(thisChildrenNode[i]); // 重調       
            }
        }else{
            return false;
        }
    }
    //匹配節點
    function matchNod");
                WriteLiteral(@"e(treeNode,num){
        var inputArea = $(""input[name='searchArea']"");
        var name = treeNode.name;
        var entityCode = treeNode.entity_code|| '';
        var val = inputArea.val(); // 獲取檢索值
        var numName = name.indexOf(val);
        var numCode = entityCode.indexOf(val);
        var num = -1;
        if( numName != -1 || numCode !=-1 ){
            num = 1;
        }
        if( numName == -1 && numCode == -1 ){
            num = -1;   
        }
        return num;
    }
    // 節點匹配成功
    function checkTrueArray(arr,treeNode){
        var thisTid = treeNode.tId;
        var thisLi = $(""#""+thisTid);
        for(var n=0;n<arr.length;n++){
            var thisNodeId = arr[n].tId;
            var thisNodeLi = $(""#""+thisNodeId);
            thisLi.show();
            thisNodeLi.show();
        }
    }
    // 節點匹配失敗
    function checkFalseArray(arr,treeNode){
        var result = [];
        var result2 = [];
        var thisTid = treeNode.tId;
        var thisLi = $(""#""+thisTid);
        var val");
                WriteLiteral(@" = inputArea.val(); // 獲取檢索值
        var thisParent =  treeNode.getParentNode();  // 獲取目標節點父節點
        if( thisParent != null ){  // 有父節點
            var thisBrotherArr =  treeNode.getParentNode().children; // 得到包含自身的兄弟陣列
            for(var m=0;m<arr.length;m++){ // 匹配父節點
                var num = matchNode(arr[m]);
                if( num != -1 ){
                    result.push(arr[m]);
                }
            }
            var resultLength = result.length;
            for( var m=0;m<thisBrotherArr.length;m++ ){ // 匹配兄弟節點
                var num = matchNode(thisBrotherArr[m]);
                if( num != -1 ){
                    result2.push(thisBrotherArr[m]);
                }
            }
            var resultLength2 = result2.length;
            // 對於自身匹配失敗的節點，要顯示必須滿足有父節點匹配成功，且兄弟級節點都匹配失敗
            if( (resultLength == 0 && resultLength2 == 0) || resultLength2 != 0 ){
                thisLi.hide();
            }
            if( resultLength !=0 && resultLength2 == 0 ){
                thisLi.s");
                WriteLiteral(@"how();
            }
        }else{
            thisLi.hide();
        }    
    }
    // 目標節點匹配失敗 目標節點即有父節點又有子節點
    function checkAllArray(arr,arr2,treeNode){
        var result = [];
        var result2 = [];
        var thisTid = treeNode.tId;
        var thisLi = $(""#""+thisTid);
        var val = inputArea.val(); // 獲取檢索值
        for(var m=0;m<arr.length;m++){ // 匹配子節點或父節點
            var num = matchNode(arr[m]);
            if( num != -1 ){
                result.push(arr[m]); // 匹配成功儲存至陣列
            }
        }
        var resultLength = result.length; // 獲取匹配成功后返回的陣列長度
        for(var m=0;m<arr2.length;m++){ // 匹配子節點或父節點
            var num = matchNode(arr2[m]);
            if( num != -1 ){
                result2.push(arr2[m]); // 匹配成功儲存至陣列
            }
        }
        var resultLength2 = result2.length; // 獲取匹配成功后返回的陣列長度
        if( resultLength == 0 && resultLength2 == 0 ){ // 子節點和父節點都匹配失敗
            thisLi.hide();
        }else{ 
            thisLi.show(); // 有一種匹配成功或都匹配成功
        }
    }
   ");
                WriteLiteral(@" function searchArea(treeId, treeNode){ // 定義搜索方法
        var inputArea = $(""input[name='searchArea']"");
        var val = inputArea.val(); // 獲取檢索值
        var treeObj=$.fn.zTree.getZTreeObj(""tree2""); // 設定根節點
        var node = treeObj.getNodes(); // 獲取根節點
        var nodes = treeObj.transformToArray(node); // 獲取所有節點
        console.log(nodes);
        for(var i=0;i<nodes.length;i++){
            var thisNodePid = nodes[i].pId;
            var thisParentNode =  
            parentArray = [];
            childArray = [];
            getParentsNode(nodes[i]); // 獲取目標節點所有父節點 返回陣列
            getChildrenNode(nodes[i]); // 獲取目標節點所有子節點 返回陣列
            var num = matchNode(nodes[i]);
            if( nodes[i].isParent == false ){ 
                if( num != -1 ){
                    checkTrueArray(parentArray,nodes[i]);
                }else{
                    checkFalseArray(parentArray,nodes[i]);
                }
            }
            if( nodes[i].isParent == true ){
                if( num != -1 ){
      ");
                WriteLiteral(@"              checkTrueArray(parentArray,nodes[i]);  
                    checkTrueArray(childArray,nodes[i]);                  
                }else{
                    checkAllArray(parentArray,childArray,nodes[i]);
                }
            }            
        }
       
    }
     // 呼叫搜索方法
        $("".searchAreaBtn"").click(function(treeId, treeNode){
            searchArea(treeId, treeNode);
        });
        var inputArea = $(""input[name='searchArea']"");
        inputArea.keyup(function(treeId, treeNode,e){
            var e = event || window.event;
            var val = inputArea.val();
            if( e.keyCode == 13 || val == """" ){
                searchArea(treeId, treeNode);
            }
        });
    </script>

");
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
