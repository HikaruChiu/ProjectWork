using AntData.ORM;
using AntData.ORM.Linq;
using Autofac.Annotation;
using Configuration;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Repository.Interface;
using ServicesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbModel;
using Dapper;
using ViewModels.Reuqest;
using System.Data;
using System.Data.SqlClient;

namespace Repository
{
    /// <summary>
    /// 系統使用者
    /// </summary>
    [Component]
    public class AccountRespository : BaseRepository<SystemUsers>, IAccountRespository
    {
        public readonly MyConnectionConfig _myConnectionConfig;
        public AccountRespository(MyConnectionConfig myConnectionConfig)
        {           
            _myConnectionConfig = myConnectionConfig;
        }

        /// <summary>
        /// 登入
        /// 2021-09-27 Hikaru 整合 Intranet User
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> LogOn(LogOnVM info)
        {
            try
            {
                if (info == null || string.IsNullOrEmpty(info.eid) || string.IsNullOrEmpty(info.pwd))
                {
                    return new Tuple<bool, string>(false, Tip.BadRequest);
                }
                                
                var existUser = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && r.Eid.Equals(info.eid));

                if (existUser == null) //SystemUser 不存在
                {
                    return new Tuple<bool, string>(false, "該帳號不存在,請確認是否有員工帳號!");
                }
                //gdyb21LQTcIANtvYMT7QVQ==                
                var pwd = CodingUtils.MD5(info.pwd);
                if (!existUser.Pwd.Equals(pwd))
                {
                    return new Tuple<bool, string>(false, "密碼錯誤!");
                }

                var systemUserAndRole = await (from u in this.Entity
                                               from ro in this.Entitys.SystemRole.Where(r => r.Tid.Equals(u.RoleTid)).DefaultIfEmpty()
                                               where u.Eid.Equals(info.eid)
                                               select new { user = u, role = ro }).FirstOrDefaultAsync();

                var systemUser = systemUserAndRole?.user;

                if (systemUser == null)
                {
                    return new Tuple<bool, string>(false, "該帳號不存在,請聯繫系統管理員!");
                }

                if (!systemUser.IsActive)
                {
                    return new Tuple<bool, string>(false, "該帳號已被禁用,請聯繫系統管理員!");
                }

                var role = systemUserAndRole.role ?? new SystemRole();

                var loginIp = WebUtils.GetClientIP();
                var userAgent = WebUtils.GetUserAgent();


                //更新
                var updateQuery = this.Entity.Where(r => r.Eid.Equals(systemUser.Eid))
                    .Set(r => r.LoginIp, loginIp)
                    .Set(r => r.LastLoginTime, DateTime.Now)
                    .Set(r => r.UserAgent, userAgent);


                //如果role不存在 也就是沒有角色 menurights卻有值 
                if (string.IsNullOrEmpty(role.RoleName) && !string.IsNullOrEmpty(systemUser.MenuRights))
                {
                    updateQuery = updateQuery.Set(r => r.MenuRights, string.Empty);
                }

                var updateResult = await updateQuery.UpdateAsync() > 0;
                if (!updateResult)
                {
                    return new Tuple<bool, string>(false, "使用者資訊更新出錯!");
                }

                //Hikaru 登入後，寫入Cookie
                WriteLoginCookie(new Token
                {
                    Code = systemUser.UserName,
                    Eid = systemUser.Eid,
                    Indx = systemUser.Indx,
                    Email = systemUser.Email,
                    MenuRights = systemUser.MenuRights,
                    DeptNo = systemUser.DeptNo,
                    RoleTid = role.Tid,
                    RoleName = role.RoleName
                    
                });
                return new Tuple<bool, string>(true, null);
            }
            catch (Exception ex)
            {
                LogHelper.Warn("login", ex);
                return new Tuple<bool, string>(false, "登入出錯");
            }
        }

        /// <summary>
        /// 2027-09-27 驗證是否為Intranet使用者
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        //public async Task<SystemUsers> LogOnByIntranet(LogOnVM info)
        //{
        //    SystemUsers users = new SystemUsers();
        //    string pwdMD5 = CodingUtils.MD5(info.pwd);
            
        //    using (IDbConnection intraConn = new SqlConnection(_myConnectionConfig.IntranetDB)) {
                                
        //        DynamicParameters dynamicParams = new();
                
        //        string strSQL = "SELECT TOP 1 * FROM administration " + 
        //            "WHERE 1=1 " +
        //            " AND deletemark = @deletemark " +
        //            " AND pwdMD5 = @pwdMD5 " +
        //            " AND (id = @account or email=@account)";
        //        dynamicParams.Add("deletemark", "N");
        //        dynamicParams.Add("pwdMD5", pwdMD5);
        //        dynamicParams.Add("account", info.eid);

        //        var data = await intraConn.QueryFirstOrDefaultAsync(strSQL, dynamicParams);
                
        //        if (data != null) {
                    
        //            //員工登入成功後
        //            //預設新增或修改到 SystemUser，給角色和選單
        //            using (IDbConnection pjwConn = new SqlConnection(_myConnectionConfig.PJW)) {

        //                //如果再Intranet有變更過密碼，但是在PJW有資料，就先刪除
        //                users = await GetUserInfo(new Token { Eid = info.eid });
        //                if (users != null) {
        //                    await pjwConn.ExecuteAsync($"DELETE system_users WHERE Tid={users.Tid}");
        //                }

        //                long defRoleId = long.Parse(ConfigHelper.GetConfig("DefaultIntraUserRoleId", "0"));

        //                //取得這個角色的選單許可權
        //                var roleInfo = await pjwConn.QueryFirstOrDefaultAsync($"SELECT Tid, MenuRights, RoleName FROM system_role WHERE Tid='{defRoleId}'");
                      
        //                foreach (var item in data) {
        //                    //如果Intranet取得到user，就新增一筆到SystemUsers
        //                    users.Eid = item.id;
        //                    users.UserName = item.first_name;
        //                    users.Phone = item.phone;
        //                    users.IsActive = true;
        //                    users.Pwd = pwdMD5;
                            
        //                    users.DataChangeLastTime = DateTime.Now;
        //                    users.CreateUser = "Intranet";

        //                    //給預設角色
        //                    foreach (var roleItem in roleInfo) {
        //                        users.MenuRights = roleItem.MenuRights;
        //                        users.RoleTid = roleItem.Tid;
        //                        users.CreateRoleName = "," + string.Join(",", roleItem.Tid) + ",";
        //                    }

        //                    /*
        //                    info.IsActive = true;
        //                    info.Pwd = !string.IsNullOrEmpty(info.Pwd) ? info.Pwd : info.Eid;
        //                    info.Pwd = CodingUtils.MD5(info.Pwd);
        //                    info.MenuRights = role.MenuRights;
        //                    info.DataChangeLastTime = DateTime.Now;
        //                    info.CreateRoleName = "," + string.Join(",", createRoleList) + ",";
        //                    info.CreateUser = user.Eid;
        //                     */


        //                    DB.Insert(users);
        //                }

        //            }
                    
        //        } 
                
        //    }

        //    return users;
        //}

        /// <summary>
        /// 獲取使用者列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Tuple<long, List<UserSM>>> GetUserList(AccountVm model, Token user)
        {
            if (model == null) { return new Tuple<long, List<UserSM>>(0, new List<UserSM>()); }

            var totalQuery = this.Entity.Where(r => r.IsActive);
            var listQuery = this.Entity.Where(r => r.IsActive);

            if (!string.IsNullOrEmpty(model.UserName))
            {
                totalQuery = totalQuery.Where(r => r.UserName.Contains(model.UserName));
                listQuery = listQuery.Where(r => r.UserName.Contains(model.UserName));
            }
            if (!string.IsNullOrEmpty(model.Eid))
            {
                totalQuery = totalQuery.Where(r => r.Eid.Contains(model.Eid));
                listQuery = listQuery.Where(r => r.Eid.Contains(model.Eid));
            }

            if (model.RoleTid > 0)
            {
                totalQuery = totalQuery.Where(r => r.RoleTid.Equals(model.RoleTid));
                listQuery = listQuery.Where(r => r.RoleTid.Equals(model.RoleTid));
            }

            var total = totalQuery.CountAsync();

            if (!GlobalSetting.SuperList.Contains(user.Eid))
            {
                //超級管理員可以檢視所有

                //只能檢視自己建立角色的所有使用者
                listQuery = listQuery.Where(r => r.CreateRoleName.Contains("," + user.RoleTid + ",") || r.RoleTid.Equals(user.RoleTid));
            }
            var userList = await
                 (from u in listQuery
                  from role in this.Entitys.SystemRole.Where(r => r.Tid.Equals(u.RoleTid)).DefaultIfEmpty()
                  select new UserSM
                  {
                      Tid = u.Tid,
                      IsActive = u.IsActive,
                      Eid = u.Eid,
                      UserName = u.UserName,
                      LoginIp = u.LoginIp,
                      RoleTid = u.RoleTid,
                      LastLoginTime = u.LastLoginTime,
                      UserAgent = u.UserAgent,
                      DataChangeLastTime = u.DataChangeLastTime,
                      RoleName = role.RoleName,
                      RoleDesc = role.Description,
                      Phone = u.Phone,
                      CreateUser = u.CreateUser
                  })
                .DynamicOrderBy(string.IsNullOrEmpty(model.OrderBy) ? "DataChangeLastTime" : model.OrderBy, model.OrderSequence)
                            .Skip((model.PageIndex - 1) * model.PageSize)
                            .Take(model.PageSize)
                            .ToListAsync();
            return new Tuple<long, List<UserSM>>(await total, userList);
        }

        /// <summary>
        /// 給使用者賦予角色
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> UserAddRole(UserAddRoleVm info)
        {
            if (info == null || info.RoleTid < 1 || info.UserTid < 1)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }
            var role = await this.Entitys.SystemRole.FirstOrDefaultAsync(r => r.Tid.Equals(info.RoleTid));
            if (role == null)
            {
                return new Tuple<bool, string>(false, Tip.RoleNotExist);
            }

            var user = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && r.Tid.Equals(info.UserTid));
            if (user == null)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            if (GlobalSetting.SuperList.Contains(user.Eid))
            {
                return new Tuple<bool, string>(false, Tip.GodUserInvaild);
            }
            var createRoleList = new List<long>();
            GetRoleName(role, createRoleList);
            createRoleList.Reverse();
            createRoleList = createRoleList.Distinct().ToList();
            var updateResult = await this.Entity.Where(r => r.IsActive && r.Tid.Equals(info.UserTid))
                 .Set(r => r.DataChangeLastTime, DateTime.Now)
                 .Set(r => r.RoleTid, info.RoleTid)
                 .Set(r => r.MenuRights, role.MenuRights)
                 .Set(r => r.CreateRoleName, "," + string.Join(",", createRoleList) + ",")
                 .UpdateAsync() > 0;

            return !updateResult ? new Tuple<bool, string>(false, Tip.UpdateError) : new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 禁用使用者
        /// </summary>
        /// <param name="userTid"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> UserDelete(long userTid)
        {
            if (userTid < 1)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            var user = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && r.Tid.Equals(userTid));
            if (user == null)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            if (GlobalSetting.SuperList.Contains(user.Eid))
            {
                return new Tuple<bool, string>(false, Tip.GodUserInvaild);
            }

            var updateResult = await this.Entity.Where(r => r.Tid.Equals(userTid))
                .Set(r => r.DataChangeLastTime, DateTime.Now)
                .Set(r => r.IsActive, false)
                .UpdateAsync() > 0;
            if (!updateResult)
            {
                return new Tuple<bool, string>(false, Tip.UpdateError);
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 更改使用者的屬性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> ChangeField(ChangeFieldVm model)
        {
            if (model == null || string.IsNullOrEmpty(model.Field) || string.IsNullOrEmpty(model.Value) || model.Tid < 1)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }
            IUpdatable<SystemUsers> updateQuery = this.Entity.Where(r => r.Tid.Equals(model.Tid))
                .Set2(model.Field, model.Value)
                .Set(r => r.DataChangeLastTime, DateTime.Now);


            var updateResult = await updateQuery.UpdateAsync() > 0;

            if (!updateResult)
            {
                return new Tuple<bool, string>(false, Tip.UpdateError);
            }
            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 手動新增使用者
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> UserAdd(SystemUsers info, Token user)
        {
            if (info == null || string.IsNullOrEmpty(info.UserName) || info.RoleTid < 1)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            info.Eid = info.Eid.ToLower();
            var existItem = await Entity.FirstOrDefaultAsync(r => r.Eid.Equals(info.Eid));
            if (existItem != null)
            {
                return new Tuple<bool, string>(false, "該員工已存在");
            }

            var role = await this.Entitys.SystemRole.FirstOrDefaultAsync(r => r.Tid.Equals(info.RoleTid));
            if (role == null)
            {
                return new Tuple<bool, string>(false, Tip.RoleNotExist);
            }

            //獲取建立者角色的包括父級的名稱列表
            var createRoleList = new List<long>();
            GetRoleName(role, createRoleList);
            createRoleList.Reverse();
            createRoleList = createRoleList.Distinct().ToList();
            info.IsActive = true;
            info.Pwd = !string.IsNullOrEmpty(info.Pwd) ? info.Pwd : info.Eid;
            info.Pwd = CodingUtils.MD5(info.Pwd);
            info.MenuRights = role.MenuRights;
            info.DataChangeLastTime = DateTime.Now;
            info.CreateRoleName = "," + string.Join(",", createRoleList) + ",";
            info.CreateUser = user.Eid;

            var inertResult = DB.Insert(info) > 0;
            return !inertResult ? new Tuple<bool, string>(false, Tip.SystemError) : new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 獲取系統使用者資訊
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<SystemUsers> GetUserInfo(Token user)
        {
            if (user == null) return null;

            return await this.Entity.Where(r => r.Eid.Equals(user.Eid)).Select(r => new SystemUsers
            {
                UserName = r.UserName,
                Phone = r.Phone,
                Tid = r.Tid
            }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 更新個人資訊
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> UpdateUserInfo(SystemUsers user)
        {
            if (user == null) return Tip.BadRequest;

            if (string.IsNullOrEmpty(user.UserName) && string.IsNullOrEmpty(user.Phone)) return Tip.BadRequest;

            var query = this.Entity.Where(r => r.Eid.Equals(user.Eid)).Set(r => r.DataChangeLastTime, DateTime.Now);

            if (!string.IsNullOrEmpty(user.UserName))
            {
                query = query.Set(r => r.UserName, user.UserName);
            }

            if (!string.IsNullOrEmpty(user.Phone))
            {
                query = query.Set(r => r.Phone, user.Phone);
            }

            var rt = await query.UpdateAsync() > 0;

            return !rt ? Tip.UpdateError : string.Empty;
        }

        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> UpdatePwd(UpdatePwdVm user)
        {
            if (user == null) return Tip.BadRequest;

            if (string.IsNullOrEmpty(user.Pwd) || string.IsNullOrEmpty(user.OldPwd)) return Tip.BadRequest;

            var currentUser = await Entity.FirstOrDefaultAsync(r => r.Eid.Equals(user.Eid));
            if (currentUser == null)
            {
                return "使用者不存在";
            }

            var pwd = CodingUtils.MD5(user.OldPwd);
            if (!currentUser.Pwd.Equals(pwd))
            {
                return "舊密碼錯誤";
            }

            var newPwd = CodingUtils.MD5(user.Pwd);
            var rt = this.Entity.Where(r => r.Eid.Equals(user.Eid)).Set(r => r.DataChangeLastTime, DateTime.Now).Set(r => r.Pwd, newPwd).Update() > 0;
            return !rt ? Tip.UpdateError : string.Empty;
        }

        private void GetRoleName(SystemRole role, List<long> roleList)
        {
            roleList.Add(role.Tid);
            if (!string.IsNullOrEmpty(role.CreateUser))
            {
                var user = this.Entity.FirstOrDefault(r => r.Eid.Equals(role.CreateUser));
                if (user != null && user.RoleTid > 0)
                {
                    var role1 = this.Entitys.SystemRole.FirstOrDefault(r => r.Tid.Equals(user.RoleTid));
                    if (role1 != null)
                    {
                        GetRoleName(role1, roleList);
                    }
                }
            }
        }

        #region WriteLoginCookie

        private string WriteLoginCookie(Token token)
        {
            string strName = GlobalSetting.CurrentLoginUserGuid;
            var value = token.ToJsonString();
            string strValue = CodingUtils.AesEncrypt(value);
            WebUtils.WriteCookie(strName, strValue);
            return value;
        }



        #endregion
    }
}
