# cms程序架构
本程序是主要是用于企业网站开发的，也可以做博客程序，程序是从之前上一篇的.net 博客程序改进过来的，主要技术由webform转成.net mvc了，由于是很早之前的项目，12年还是mvc3版本，当然还是跑在linux下的。

## 技术栈
- .net framework 4.0
- sqlite 数据库
- mono linux 运行环境以及mono下的sqlite库
- razor 模板引擎
- dapper 轻量级orm框架
- vs2017 社区版本

这次的开发工具比较新了吧，上次用vs2010发的，跨度比较大，这个项目之前也是在10下开发出来的，虽然用2017，其实没什么影响的。

razor引擎比nvelocity的易用性高很多，而且跟后端集合的比较好。

而且这次的项目完全使用dapper orm，整个数据库访问层操作看起来也清爽很多了。

## 代码结构
![代码结构](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224162331443-281105605.png)
左侧主要分为三个文件夹，Jqpress.web是存放web路由入口、模板皮肤、静态文件，上传文件夹，后台管理程序通过Areas域管理来实现的，整个项目结构还是比较清晰的。


## 路由设计
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224162657959-398039244.png)

路由入口为HomeController这个文件，包括首页、产品列表，文章列表，文章内容等待，都是通过这个Controller来路由的，虽然功能比较多，但是代码量不到300行，业务逻辑处理基本都在service层。

如下为一个列表界面的Action逻辑才20几行

``` C#
        public ActionResult Category(string pagename) 
        {
            var model = new PostListModel();
            CategoryInfo cate = _categoryService.GetCategory(pagename);
            model.Category = cate;
            if (cate != null)
            {
                int categoryId = cate.CategoryId;
                model.MetaKeywords = cate.CateName;
                model.MetaDescription = cate.Description;
                ViewBag.Title = cate.CateName;
                model.Url = ConfigHelper.SiteUrl + "category/" + Jqpress.Framework.Utils.StringHelper.SqlEncode(pagename) + "/page/{0}";

                const int pageSize = 10;
                int count = 0;
                int pageIndex = PressRequest.GetInt("page", 1);
                int cateid = PressRequest.GetQueryInt("cateid", -1);
                int tagid = PressRequest.GetQueryInt("tagid", -1);
                if (cateid > 0)
                    pageIndex = pageIndex + 1;
                var cateids =categoryId+","+ _categoryService.GetCategoryList().FindAll(c => c.ParentId == categoryId).Aggregate(string.Empty, (current, t) => current + (t.CategoryId + ",")).TrimEnd(',');
                var postlist = _postService.GetPostPageList(pageSize, pageIndex, out count, cateids.TrimEnd(','), tagid, -1, -1, -1, -1, -1,-1, "", "", "");
                model.PageList.LoadPagedList(postlist);
                model.PostList = (List<PostInfo>)postlist;
            }
            model.IsDefault = 0;
            

            return View(model.Category.ViewName,model);
        }
```

## 模板引擎设计
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224163243584-1312562996.png)


![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224173006568-1791258772.png)


模板引擎采用razor，不同的cms风格模板以文件夹的形式存放在Themes文件夹下,通过后端设置可以随意切换模板（此功能还在完善当中）。



## 数据存储设计
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224163735615-568447981.png)

作为轻量级的cms，当然不会用mysql或者sqlserver，依然坚定的使用sqlite，以上是通过dapper进行数据库操作，虽然没有微软增加的orm强大，但是可读性和操控性都是非常好的。

![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224164701459-27334103.png)

因为要支持linux，所以sqlite的操作类需要`using Mono.Data.Sqlite;` 
，当然可以在windows下切换过来，上面我写的切换方式太粗暴了，学过设计模式的同学应该分分钟能够重构出来，切换windows和linux简直不要太方便。


# 数据库结构
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224170703350-264992807.png)


- `jq_category` 分类及标签表
- `jq_comments` 评论表
- `jq_links` 友链及导航设置
- `jq_posts` 文章表
- `jq_sites` 站点访问统计数字及文章、分类、数量统计表
- `jq_users` 账户表

数据库结构跟之前的 博客程序没差别，只是部分表结构字段有所差别

# 运行
## vs2017调试模式预览

cms首页
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224165016850-1657811660.png)



后台登录，默认用户名admin，密码123456
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224165218506-2007036355.png)


后台首页
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224165352162-1305665329.png)

文章列表
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224165456928-232080569.png)

文章编辑
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224165633771-1635828495.png)



## linux下的部署方式

## mono
![mono](http://images2017.cnblogs.com/blog/94489/201712/94489-20171209224739669-240915517.png)
上图为我服务器上的mono安装信息。


## jexus
linux下需要安装mono和jexus就可以运行起来，mono作为.net framework的linux运行环境，jexus作为web服务器。


![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224180219365-500707975.png)

jexus配置，因为我的服务器上还有其他语言的站点，所以没有直接采用jexus对外服务。


## nginx
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224180511256-524879828.png)

nginx代理了jexus的81端口，此地方不是必选，但是Nginx作为常规的代理软件，可以在服务器上跑.net、php、java等，各司其职，百花齐放。


## bin文件夹说明
![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224174826146-393656298.png)
.net mvc3在 linux下的部署需要注意的是需要将项目下引用的dll都需要上传到bin目录下。并且还需要上传Microsoft.web.Infrastructure.dll，这个是发布mvc项目需要用的。可以对比下我webform下的Linux博客程序站点bin下只要引入Mono和Nvelocity这2个第三方dll，其他都是自己的业务程序，这个.net cms要的东西还挺多,下图是我的博客站点下的bin文件夹内容。


![](http://images2017.cnblogs.com/blog/94489/201712/94489-20171224175257100-614022173.png)


## linux服务器上的的.net 程序问题
- jexus不支持中文，所以文件上传的路径必须重写成字母加数字符号形式，
- jexus web服务器是默认对大小写敏感的，所以部署的时候一定要
只需要把 jws这个脚本文件中的 “export MONO_IOMAP=...”这一句前边的“#”去掉，就可以不区分大小写了

# 源码获取
[https://github.com/robotbird/jqpress.cms](https://github.com/robotbird/jqpress.cms)

为了防止部分同学无法访问Github，所以放oschina的gitee上了

[https://gitee.com/robotbird/jqpress.cms](https://gitee.com/robotbird/jqpress.cms)

# 后记

在开发这个cms的时候那时候.net core还没诞生，作为linux下的.net mvc项目还还是比较新颖的，只可惜没能好好的写代码，跑去做项目做产品去了，在这里开源出来缅怀写代码的岁月，如果对大家有用那最好不过了。
如果对源码感兴趣可以联系qq:330296409

博客地址:http://www.cnblogs.com/jqbird/p/8098334.html