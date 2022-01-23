@{
    ViewBag.Title = "{{ModelName}}";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<div class="wrapper wrapper-content animated fadeInRight">
    <div class="ibox float-e-margins">
        <div class="ibox-content">
            <div class="row row-lg">
                <div class="col-sm-12">
                    <div>
                        <div>
                            <div class="btn-group hidden-xs" id="{{ModelClassName}}EventsToolbar" role="group">
								<!--<input placeholder="輸入openID" class="ibox-input" v-on:keyup.enter="_FreshTable" id="openID" style="margin-left: 10px;">-->
                                <button type="button" action-id="{{ModelClassName}}-add" action-name="新增{{ModelName}}" class="btn btn-success authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Add">新增{{ModelName}}</button>
                                <button type="button" action-id="{{ModelClassName}}-update" action-name="修改{{ModelName}}" class="btn btn-primary authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Update">修改{{ModelName}}</button>
                                <button type="button" action-id="{{ModelClassName}}-delete" action-name="刪除{{ModelName}}" class="btn btn-danger authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Delete">刪除{{ModelName}}</button>
								<button type="button" action-id="{{ModelClassName}}-export" action-name="導出{{ModelName}}" class="btn btn-navigate authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Export">導出{{ModelName}}</button>
                                <button type="button" action-id="{{ModelClassName}}-import" action-name="匯入{{ModelName}}" class="btn btn-navigate authorization" style="margin-right: 10px;display:none" v-on:click="_{{ModelClassName}}Import">匯入{{ModelName}}</button>
                            </div>
                            <table id="{{ModelClassName}}Table" data-side-pagination="server" data-sort-order="desc" data-mobile-responsive="true"></table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>



<div class="modal inmodal" id="myModal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 v-if="add" class="modal-title">新增{{ModelName}}</h4>
                <h1 v-else class="modal-title" style="color: red">修改{{ModelName}}</h1>
            </div>
            <div class="modal-body">
			{% for field in ModelFields %}
				{% if field.Name == 'IsActive' %}
			<div class="form-group">
				<label class="control-label">{{field.Comment}}：&nbsp;&nbsp;&nbsp;</label>
				<input v-model="currentRow.{{field.Name}}" class="js-switch" type="checkbox" id="{{field.Name}}" />&nbsp;&nbsp;{{currentRow.{{field.Name}} ? "是" : "否"}}
			</div>
				{% else %}
			<div class="form-group">
				<label class="control-label">{{field.Comment}}：</label>
				<input v-model="currentRow.{{field.Name}}" class="form-control" type="text" placeholder="請輸入{{field.Comment}}">
			</div>
				{% endif %}
			{% endfor %}
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-white" id="close-model" v-on:click="_Close">關閉</button>
                <button type="button" class="btn btn-primary" id="save-model" v-on:click="_Save">提交</button>
            </div>
        </div>
    </div>
</div>
 
 <div class="modal inmodal" id="uploadModal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">匯入{{ModelName}}</h4>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label class="control-label">選擇Excel：</label>
                    <input id="upload" type="file" class="file" data-show-upload="false" />
                </div>
                <div class="form-group">
                    <a href="~/{{ModelClassName}}/ExcelTemplete">下載Excel模板</a>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-white" v-on:click="_Close">關閉</button>
                <button type="button" class="btn btn-primary" v-on:click="_SaveImport">提交</button>
            </div>
        </div>
    </div>
</div>

 @section Scripts{ 
		<script type="text/javascript">

		var vm = new Vue({
			el: 'body',
			data: {
				add: true,
				currentRow: {}
			},
			computed: {},
			ready:function(){
				auth();
				//initSwitchery();
				OnReady();
			},
			methods: {

				_{{ModelClassName}}Add: function () {
					vm.add = true;
					$('#myModal').modal({ backdrop: 'static', keyboard: false });
					$('#myModal').modal('show');
				},
				_FreshTable: function () {
					try {
						$(".bootstrap-table button[name='refresh']")[0].click();
					} catch (e) {

					}
				},
				_Close: function () {
					vm.currentRow = {};
					$('#myModal').modal('hide');

				    $("#upload").val("");
                    $('.fileinput-remove-button') && $('.fileinput-remove-button').click();
                    $('#uploadModal').modal('hide');
				},
				_Save: function () {
					//if (!vm.currentRow.Value) {
					//    swal({
					//        title: "",
					//        text: "請輸入描述！",
					//        type: "error"
					//    });
					//    return;
					//}
               
					QQT.ajax('/{{ModelClassName}}/Add{{ModelClassName}}', 'Post', vm.currentRow)
						.done(function (response) {
							$('#{{ModelClassName}}Table').bootstrapTable('refresh', { silent: true });
							swal("成功啦！", "操作成功!", "success");
							vm._Close();
						});
				},
				_{{ModelClassName}}Update: function () {
					vm.add = false;
					var selectRow = $('#{{ModelClassName}}Table').bootstrapTable('getSelections');
					if (selectRow.length < 1) {
						swal({
							title: "",
							text: "請選擇！",
							type: "error"
						});
						return;
					}
					vm.currentRow = selectRow[0];
					$('#myModal').modal({ backdrop: 'static', keyboard: false });
					$('#myModal').modal('show');
				},
				_{{ModelClassName}}Delete: function () {
					var selectRow = $('#{{ModelClassName}}Table').bootstrapTable('getSelections');
					if (selectRow.length < 1) {
						swal({
							title: "",
							text: "請選擇！",
							type: "error"
						});
						return;
					}
					var tid = selectRow[0].Tid;
					swal({
							title: "您確定要刪除嗎?",
							text: "請謹慎操作！",
							type: "warning",
							showCancelButton: true,
							confirmButtonColor: "#DD6B55",
							confirmButtonText: "是的，朕決心已定！",
							cancelButtonText: "讓朕再考慮一下…",
							showLoaderOnConfirm: true,
							preConfirm: function () {
								return QQT.ajax('/{{ModelClassName}}/Del{{ModelClassName}}','Post',{tid:tid});                
							}
                    }).then(function (response) {
                        $('#{{ModelClassName}}Table').bootstrapTable('refresh', { silent: true });
                        swal("成功啦！", "操作成功!", "success");
                    }).catch(function() {
                    
                    });
				},
                _SaveImportImport: function() {
					if ($('#upload').val().length < 1) {
                        swal({
                            title: "",
                            text: "請選擇Excel檔案！",
                            type: "error"
                        });
                        return;
                    }
                    var data = new FormData();
                    var files = $("#upload").get(0).files;
                    data.append("UploadedFile", files[0]);
                    QQT.ajax('/{{ModelClassName}}/Upload', 'Post', data, false, false, true)
                        .done(function (response) {
                            vm._Close();
                            swal("成功啦！", response.Info, "success");
                            $('#{{ModelClassName}}Table').bootstrapTable('refresh', { silent: true });
                        });
                },
                _{{ModelClassName}}Import: function() {
                      $('#uploadModal').modal({ backdrop: 'static', keyboard: false });
                    $('#uploadModal').modal('show');
                },
                _{{ModelClassName}}Export: function() {
                    $.fileDownload(window.appUrl + '/{{ModelClassName}}/Export',
                        {
                            httpMethod: 'POST',
                            dataType: "json",
                            contentType: "application/json",
                            data: getParam(),
                            preparingMessageHtml: "正在下載中,請稍後...",
                            failMessageHtml: "下載出錯！"
                        });
                    return false;
                }
			}
		});

		function OnReady(){
			$('#{{ModelClassName}}Table').bootstrapTable({
				ajax: ajaxRequest,
				pageNumber: 1,
				pageSize: 10,
				pageList: [10, 25, 50, 100],
				resetOffset: true,
				search: false,
				sortable: true,
				pagination: true,
				height: $(window).height(),
				showRefresh: true,
				showToggle: true,
				showColumns: false,
				striped: true,
				sortOrder: 'desc',
				clickToSelect: true,
				singleSelect: true,
				cache: false,
				// showPaginationSwitch:true,
				dataType: "json",
				iconSize: 'outline',
				toolbar: '#{{ModelClassName}}EventsToolbar',
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
				{% for field in ModelFields %}
					{
						field: '{{field.Name}}',
						title: '{{field.Comment}}',
						sortable: true
					},
				{% endfor %}
                   
				],
				onLoadSuccess: function () {
					tipReCreate();
				},
				onToggle: function () {
					tipReCreate();
				},
				onEditableSave: function (field, row, oldValue, $el) {

				},
				rowStyle: function (row, index) {
					//這裡有5個取值代表5中顏色['active', 'success', 'info', 'warning', 'danger'];
					var strclass = "";
					return {};
					//return { classes: strclass }
				}
			});

			$(window).resize(function () {
				$('#{{ModelClassName}}Table').bootstrapTable('resetView', { height: $(window).height() });
				tipReCreate();
			});
		}

		function tipReCreate() {
			setTimeout(function () {
				Tipped.create('.tip_infomation',
					function (element) {
						return "<strong>" + $(element).data('content') + "</strong>";
					});
			}, 200);
		}

		function LongStringFormatter(value, row, index) {
			if (value.length <= 15) {
				return value;
			}
			else {
				return "<div class='tip_infomation' data-content='" + QQT.InsertEnter(value, 10) + "'>" + value.substring(0, 15) + "...</div>";
			}
		}

        function getParam() {
            //這裡可以寫你的其他篩選條件欄位
            return {
                //xx: $.trim($('#yyyy').val())
            }
        }

		function ajaxRequest(params) {
			var pageSize = params.data.limit;
			var pageIndex = params.data.offset / params.data.limit + 1;
			var orderBy = params.data.sort;
			var orderSequence = params.data.order;
			QQT.ajax('/{{ModelClassName}}/Get{{ModelClassName}}List',
				'POST',
				$.extend(getParam(), {
                    pageIndex: pageIndex,
                    pageSize: pageSize,
                    orderBy: orderBy,
                    orderSequence: orderSequence,
                }))
			.done(function (response) {
				params.success({
					total: response.Total,
					rows: response.Rows
				});
			});
		}


	</script>
}