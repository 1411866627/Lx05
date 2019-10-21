using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;


namespace DAL
{
    public class Activity
    {
        private Activity() { }
        private static Activity _instance = new Activity();
        public static Activity Instance
        {
            get
            {
                return _instance;
            }
        }

        string cns = AppConfigurtaionServices.Configuration.GetConnectionString("cns");
        public Model.Activity GetModel(int id)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select activityId,activityName,endTime,activityPicture,summary,activityVerify,userName,recommend,recommendtime,activityIntroduction,endTime>=now() as activityStatus from activity where activityId=@id";
                return cn.QueryFirstOrDefault<Model.Activity>(sql, new { id = id });
            }
        }
        public int GetVerifyCount()  //获取审核通过活动个数
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select count(1) from activity where activityVerify='审核通过'";
                return cn.ExecuteScalar<int>(sql);
            }
        }
        public IEnumerable<Model.Activity> GetVerifyPage(Model.Page page) //返回审核通过活动第n页数据
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "with a as(select row_number() over(order by endTime desc) as num, activityId,activityName,endTime,activityPicture,summary,activityVerify,userName,recommend,recommendtime,endTime>=now() as activityStatus from activity where activityVerify='审核通过')";
                sql += "select* from a where num between (@pageIndex-1)*@pageSize+1 and @pageIndex*@pageSize;";
                return cn.Query<Model.Activity>(sql, page);
            }
        }
        public IEnumerable<Model.Activity> GetNew()  //返回2个最新、审核通过并且未过期活动
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select * from activity where activityVerify='审核通过' and endTime>=now() order by endTime desc limit 2";
                return cn.Query<Model.Activity>(sql);
            }
        }
        public Model.Activity GetRecommend()  //返回1个最新、审核通过、未过期并且推荐（recommend）的作品
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select * from activity where activityVerify='审核通过' and recommend='是' and endTime>=now() order by recommendTime desc limit 1 ";
                return cn.QueryFirstOrDefault<Model.Activity>(sql);
            }
        }
        public Model.Activity GetEnd()  //返回最新的1个已过期活动
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select * from activity where activityVerify='审核通过' and endTime<now() order by endTime desc limit 1 ";
                return cn.QueryFirstOrDefault<Model.Activity>(sql);
            }
        }

        public IEnumerable<Model.ActivityName> GetActivityNames()  //返回所有活动id和名称
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select activityId,activityName from activity";
                return cn.Query<Model.ActivityName>(sql);
            }
        }
        public int GetCount()  //返回活动个数
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select count(1) from activity";
                return cn.ExecuteScalar<int>(sql);
            }
        }
        public IEnumerable<Model.ActivityNo> GetPage(Model.Page page)  //返回第n页活动数据
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "with a as(select row_number() over(order by endTime desc) as num, activity.* from activity)";
                sql += "select* from a where num between (@pageIndex-1)*@pageSize+1 and @pageIndex*@pageSize;";
                return cn.Query<Model.ActivityNo>(sql, page);
            }
        }
        public int Add(Model.Activity active)  //添加新活动并返回id
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "insert into activity(activityname,endtime,activitypicture,activityintroduction,summary,activityverify,activitystatus,username,recommend) values(@activityName,@endTime,@activityPicture,@activityIntroduction,@summary,@activityVerify,@activityStatus,@userName,@recommend);"; 
                sql += "SELECT @@IDENTITY";
                return cn.ExecuteScalar<int>(sql, active);
            }
        }
        public int Update(Model.Activity active)  //修改指定id的活动
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "update activity set activityname=@activityName,endtime=@endTime,activitypicture=@activityPicture,activityintroduction=@activityIntroduction,summary=@summary,activityverify=@activityVerify,activitystatus=@activityStatus where activityid=@activityId";  
                return cn.Execute(sql, active);
            }
        }
        public int UpdateImg(Model.Activity active)  //修改指定id的活动图片
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "update activity set activitypicture=@activityPicture where activityid=@activityId";
                return cn.Execute(sql, active);
            }
        }
        public int UpdateVerify(Model.Activity active)  //修改指定id的审核情况
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "update activity set activityVerify=@activityVerify where activityid=@activityId";
                return cn.Execute(sql, active);
            }
        }
        public int Delete(int id) //删除活动
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "delete from activity where activityid=@id";
                return cn.Execute(sql, new { id = id });
            }
        }
        public int UpdateRecommend(Model.Activity activity)  //修改活动推荐情况（是否推荐）
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "update activity set recommend=@recommend,recommendTime=@recommendTime where activityid=@activityId";
                return cn.Execute(sql, activity);
            }
        }


    }
}
