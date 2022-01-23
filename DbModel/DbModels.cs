using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModel
{
	/// <summary>
	/// Visx Version   : 3.8
	/// Github         : https://github.com/yuzd/AntData.ORM
	/// Database       : PJW
	/// Data Source    : localhost
	/// Server Version : 5.7.26
	/// </summary>
	public partial class AntEntity : IEntity
	{
		/// <summary>
		/// 系統選單表
		/// </summary>
		public IQueryable<SystemMenu> SystemMenu { get { return this.Get<SystemMenu>(); } }
		/// <summary>
		/// 選單按鈕
		/// </summary>
		public IQueryable<SystemPageAction> SystemPageAction { get { return this.Get<SystemPageAction>(); } }
		/// <summary>
		/// 角色表
		/// </summary>
		public IQueryable<SystemRole> SystemRole { get { return this.Get<SystemRole>(); } }
		/// <summary>
		/// 後臺系統使用者表
		/// </summary>
		public IQueryable<SystemUsers> SystemUsers { get { return this.Get<SystemUsers>(); } }
				
		private readonly DataConnection con;

		public DataConnection DbContext
		{
			get { return this.con; }
		}

		public IQueryable<T> Get<T>()
			 where T : class
		{
			return this.con.GetTable<T>();
		}

		public IAntTable<T, T2> Get<T, T2>()
			 where T2 : new()
		{
			return this.con.GetTable<T, T2>();
		}

		public AntEntity(DataConnection con)
		{
			this.con = con;
		}
	}


	/// <summary>
	/// 系統選單表
	/// </summary>
	[Table(Db = "PJW", Comment = "系統選單表", Name = "system_menu")]
	public partial class SystemMenu : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// MenuId
		/// </summary>
		[Column("Tid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "MenuId"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 最後更新時間
		/// </summary>
		[Column("DataChangeLastTime", DataType = AntData.ORM.DataType.DateTime, Comment = "最後更新時間"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive", DataType = AntData.ORM.DataType.Boolean, Precision = 3, Scale = 0, Comment = "是否可用"), NotNull]
		public virtual bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 父節點Id
		/// </summary>
		[Column("ParentTid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "父節點Id"), NotNull]
		public virtual long ParentTid { get; set; } // bigint(20)

		/// <summary>
		/// 名稱
		/// </summary>
		[Column("Name", DataType = AntData.ORM.DataType.VarChar, Length = 50, Comment = "名稱"), Nullable]
		public virtual string Name { get; set; } // varchar(50)

		/// <summary>
		/// 展示的圖示
		/// </summary>
		[Column("Ico", DataType = AntData.ORM.DataType.VarChar, Length = 100, Comment = "展示的圖示"), Nullable]
		public virtual string Ico { get; set; } // varchar(100)

		/// <summary>
		/// 連線地址
		/// </summary>
		[Column("Url", DataType = AntData.ORM.DataType.VarChar, Length = 200, Comment = "連線地址"), Nullable]
		public virtual string Url { get; set; } // varchar(200)

		/// <summary>
		/// 排序
		/// </summary>
		[Column("OrderRule", DataType = AntData.ORM.DataType.Int32, Precision = 10, Scale = 0, Comment = "排序"), Nullable]
		public virtual int? OrderRule { get; set; } // int(11)

		/// <summary>
		/// 等級
		/// </summary>
		[Column("Level", DataType = AntData.ORM.DataType.Int32, Precision = 10, Scale = 0, Comment = "等級"), Nullable]
		public virtual int? Level { get; set; } // int(11)

		/// <summary>
		/// 樣式
		/// </summary>
		[Column("Class", DataType = AntData.ORM.DataType.VarChar, Length = 100, Comment = "樣式"), Nullable]
		public virtual string Class { get; set; } // varchar(100)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = DateTime.Now;

		#endregion
	}

	/// <summary>
	/// 選單按鈕
	/// </summary>
	[Table(Db = "PJW", Comment = "選單按鈕", Name = "system_page_action")]
	public partial class SystemPageAction : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// 主鍵
		/// </summary>
		[Column("Tid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "主鍵"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 最後更新時間
		/// </summary>
		[Column("DataChangeLastTime", DataType = AntData.ORM.DataType.DateTime, Comment = "最後更新時間"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 訪問路徑
		/// </summary>
		[Column("MenuTid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "訪問路徑"), NotNull]
		public virtual long MenuTid { get; set; } // bigint(20)

		/// <summary>
		/// ActionId
		/// </summary>
		[Column("ActionId", DataType = AntData.ORM.DataType.VarChar, Length = 100, Comment = "ActionId"), Nullable]
		public virtual string ActionId { get; set; } // varchar(100)

		/// <summary>
		/// ActionName
		/// </summary>
		[Column("ActionName", DataType = AntData.ORM.DataType.VarChar, Length = 255, Comment = "ActionName"), Nullable]
		public virtual string ActionName { get; set; } // varchar(255)

		/// <summary>
		/// ControlName
		/// </summary>
		[Column("ControlName", DataType = AntData.ORM.DataType.VarChar, Length = 255, Comment = "ControlName"), Nullable]
		public virtual string ControlName { get; set; } // varchar(255)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = DateTime.Now;

		#endregion
	}

	/// <summary>
	/// 角色表
	/// </summary>
	[Table(Db = "PJW", Comment = "角色表", Name = "system_role")]
	public partial class SystemRole : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// 最後更新時間
		/// </summary>
		[Column("DataChangeLastTime", DataType = AntData.ORM.DataType.DateTime, Comment = "最後更新時間"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 角色名稱
		/// </summary>
		[Column("RoleName", DataType = AntData.ORM.DataType.VarChar, Length = 100, Comment = "角色名稱"), Nullable]
		public virtual string RoleName { get; set; } // varchar(100)

		/// <summary>
		/// 描述
		/// </summary>
		[Column("Description", DataType = AntData.ORM.DataType.VarChar, Length = 200, Comment = "描述"), Nullable]
		public virtual string Description { get; set; } // varchar(200)

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive", DataType = AntData.ORM.DataType.Boolean, Precision = 3, Scale = 0, Comment = "是否可用"), NotNull]
		public virtual bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 選單許可權
		/// </summary>
		[Column("MenuRights", DataType = AntData.ORM.DataType.VarChar, Length = 150, Comment = "選單許可權"), Nullable]
		public virtual string MenuRights { get; set; } // varchar(150)

		/// <summary>
		/// 主鍵
		/// </summary>
		[Column("Tid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "主鍵"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 按鈕等許可權
		/// </summary>
		[Column("ActionList", DataType = AntData.ORM.DataType.Text, Length = 4294967295, Comment = "按鈕等許可權"), Nullable]
		public virtual string ActionList { get; set; } // longtext

		/// <summary>
		/// 建立者
		/// </summary>
		[Column("CreateUser", DataType = AntData.ORM.DataType.VarChar, Length = 20, Comment = "建立者"), Nullable]
		public virtual string CreateUser { get; set; } // varchar(20)

		/// <summary>
		/// 建立者的角色Tid
		/// </summary>
		[Column("CreateRoleTid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "建立者的角色Tid"), NotNull]
		public virtual long CreateRoleTid { get; set; } // bigint(20)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = DateTime.Now;

		#endregion
	}

	/// <summary>
	/// 後臺系統使用者表
	/// </summary>
	[Table(Db = "PJW", Comment = "系統使用者表", Name = "system_users")]
	public partial class SystemUsers : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// 主鍵
		/// </summary>
		[Column("Tid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "主鍵"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 最後更新時間
		/// </summary>
		[Column("DataChangeLastTime", DataType = AntData.ORM.DataType.DateTime, Comment = "最後更新時間"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive", DataType = AntData.ORM.DataType.Boolean, Precision = 3, Scale = 0, Comment = "是否可用"), NotNull]
		public virtual bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 登入名
		/// </summary>
		[Column("Eid", DataType = AntData.ORM.DataType.VarChar, Length = 36, Comment = "登入名"), Nullable]
		public virtual string Eid { get; set; } // varchar(36)

		/// <summary>
		/// 使用者名稱
		/// </summary>
		[Column("UserName", DataType = AntData.ORM.DataType.VarChar, Length = 50, Comment = "使用者名稱"), Nullable]
		public virtual string UserName { get; set; } // varchar(50)

		/// <summary>
		/// 密碼
		/// </summary>
		[Column("Pwd", DataType = AntData.ORM.DataType.VarChar, Length = 50, Comment = "密碼"), Nullable]
		public virtual string Pwd { get; set; } // varchar(50)

		/// <summary>
		/// EMail
		/// </summary>
		[Column("Email", DataType = AntData.ORM.DataType.VarChar, Length = 100, Comment = "Email"), Nullable]
		public virtual string Email { get; set; } // varchar(100)

		/// <summary>
		/// 手機號
		/// </summary>
		[Column("Phone", DataType = AntData.ORM.DataType.VarChar, Length = 20, Comment = "手機號"), Nullable]
		public virtual string Phone { get; set; } // varchar(20)

		/// <summary>
		/// 登入IP
		/// </summary>
		[Column("LoginIp", DataType = AntData.ORM.DataType.VarChar, Length = 30, Comment = "登入IP"), Nullable]
		public virtual string LoginIp { get; set; } // varchar(30)

		/// <summary>
		/// 選單許可權
		/// </summary>
		[Column("MenuRights", DataType = AntData.ORM.DataType.VarChar, Length = 150, Comment = "選單許可權"), Nullable]
		public virtual string MenuRights { get; set; } // varchar(150)

		/// <summary>
		/// 角色Tid(一個人只有一個角色)
		/// </summary>
		[Column("RoleTid", DataType = AntData.ORM.DataType.Int64, Precision = 19, Scale = 0, Comment = "角色Tid(一個人只有一個角色)"), NotNull]
		public virtual long RoleTid { get; set; } // bigint(20)

		/// <summary>
		/// 最後登入系統時間
		/// </summary>
		[Column("LastLoginTime", DataType = AntData.ORM.DataType.DateTime, Comment = "最後登入系統時間"), Nullable]
		public virtual DateTime? LastLoginTime { get; set; } // datetime

		/// <summary>
		/// 登入的瀏覽器資訊
		/// </summary>
		[Column("UserAgent", DataType = AntData.ORM.DataType.VarChar, Length = 500, Comment = "登入的瀏覽器資訊"), Nullable]
		public virtual string UserAgent { get; set; } // varchar(500)

		/// <summary>
		/// 建立的角色名稱
		/// </summary>
		[Column("CreateRoleName", DataType = AntData.ORM.DataType.VarChar, Length = 500, Comment = "建立的角色名稱"), Nullable]
		public virtual string CreateRoleName { get; set; } // varchar(500)

		/// <summary>
		/// 建立者
		/// </summary>
		[Column("CreateUser", DataType = AntData.ORM.DataType.VarChar, Length = 50, Comment = "建立者"), Nullable]
		public virtual string CreateUser { get; set; } // varchar(50)

		/// <summary>
		/// Intranet的 administration編號 indx
		/// </summary>
		[Column("Indx", DataType = AntData.ORM.DataType.Int64, Comment = "Indx"), Nullable]
		public virtual string Indx { get; set; }
		/// <summary>
		/// Intranet的 administration department_id
		/// </summary>
		[Column("DeptNo", DataType = AntData.ORM.DataType.Int64, Comment = "DeptNo"), Nullable]
		public virtual string DeptNo { get; set; }


		#endregion

		#region Field

		private DateTime _DataChangeLastTime = DateTime.Now;

		#endregion
	}

	public static partial class TableExtensions
	{
		public static SystemMenu FindByBk(this IQueryable<SystemMenu> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemMenu> FindByBkAsync(this IQueryable<SystemMenu> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static SystemPageAction FindByBk(this IQueryable<SystemPageAction> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemPageAction> FindByBkAsync(this IQueryable<SystemPageAction> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static SystemRole FindByBk(this IQueryable<SystemRole> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemRole> FindByBkAsync(this IQueryable<SystemRole> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static SystemUsers FindByBk(this IQueryable<SystemUsers> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemUsers> FindByBkAsync(this IQueryable<SystemUsers> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}
	}
}
