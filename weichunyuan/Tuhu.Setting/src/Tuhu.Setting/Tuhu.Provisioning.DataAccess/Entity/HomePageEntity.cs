using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{

    /// <summary>
    /// 首页列表
    /// </summary>
    public class SE_HomePageConfig
    {

        public SE_HomePageConfig()
        {

        }


        #region Model
        private int _id;
        private string _homepagename;
        private bool _isenabled;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string HomePageName
        {
            set { _homepagename = value; }
            get { return _homepagename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set { _isenabled = value; }
            get { return _isenabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }


        public int? TypeConfig { get; set; }


        public IEnumerable<SE_HomePageModuleConfig> ModuleItems { get; set; }

        #endregion Model

        /// <summary>
        /// 开始版本号
        /// </summary>
        public string StartVersion { get; set; }

        /// <summary>
        /// 结束版本号
        /// </summary>
        public string EndVersion { get; set; }

    }

    /// <summary>
    /// 首页模块信息表
    /// </summary>
    public class SE_HomePageModuleConfig
    {
        public SE_HomePageModuleConfig()
        {

        }

        #region Model

        public string BgColor { get; set; }

        private int _id;
        private int? _fkhomepage;
        private string _modulename;
        private int? _modeultype;
        private bool _isenabled;
        private int? _prioritylevel;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime;
        private string _spliteline;
        private string _bgimageurl;
        private string _fontcolor;
        private string _startversion;
        private string _endversion;
        private string _title;
        private string _titleimageurl;
        private string _titlecolor;
        private bool _ismore;
        private string _moreuri;
        private bool _istag;
        private string _tagcontent;
        private bool _ischildmodule;
        private bool _ismorechannel;
        private bool _ismorecity;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? FKHomePage
        {
            set { _fkhomepage = value; }
            get { return _fkhomepage; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ModuleName
        {
            set { _modulename = value; }
            get { return _modulename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ModuleType
        {
            set { _modeultype = value; }
            get { return _modeultype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set { _isenabled = value; }
            get { return _isenabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? PriorityLevel
        {
            set { _prioritylevel = value; }
            get { return _prioritylevel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SpliteLine
        {
            set { _spliteline = value; }
            get { return _spliteline; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BgImageUrl
        {
            set { _bgimageurl = value; }
            get { return _bgimageurl; }
        }

        public string FileUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FontColor
        {
            set { _fontcolor = value; }
            get { return _fontcolor; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string StartVersion
        {
            set { _startversion = value; }
            get { return _startversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EndVersion
        {
            set { _endversion = value; }
            get { return _endversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TitleImageUrl
        {
            set { _titleimageurl = value; }
            get { return _titleimageurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TitleColor
        {
            set { _titlecolor = value; }
            get { return _titlecolor; }
        }


        public string TitleBgColor { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool IsMore
        {
            set { _ismore = value; }
            get { return _ismore; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MoreUri
        {
            set { _moreuri = value; }
            get { return _moreuri; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsTag
        {
            set { _istag = value; }
            get { return _istag; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TagContent
        {
            set { _tagcontent = value; }
            get { return _tagcontent; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsChildModule
        {
            set { _ischildmodule = value; }
            get { return _ischildmodule; }
        }
        /// <summary>
        /// 1是普通;2是动图
        /// </summary>
        public string ImageType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsMoreChannel
        {
            set { _ismorechannel = value; }
            get { return _ismorechannel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsMoreCity
        {
            set { _ismorecity = value; }
            get { return _ismorecity; }
        }

        /// <summary>
        /// 动画模块的排版 1111 112 121 211 22
        /// </summary>
        public int? Pattern { get; set; }

        public int? Margin { get; set; }

        public string UriCount { get; set; }

        public IEnumerable<SE_HomePageModuleHelperConfig> ModuleHelper { get; set; }

        #endregion Model

        /// <summary>
        /// 是否是新人
        /// </summary>
        public int? IsNewUser { get; set; }
    }


    /// <summary>
    /// 模块的辅助模块
    /// </summary>
    public class SE_HomePageModuleHelperConfig
    {
        public SE_HomePageModuleHelperConfig()
        {

        }

        #region Model
        /// <summary>
        /// 1是普通;2是动图
        /// </summary>
        public string ImageType { get; set; }
        public string BgColor { get; set; }

        public string FileUrl { get; set; }
        private int _id;
        private int? _fkhomepagemoduleid;
        private string _modulename;
        private int? _modeultype;
        private bool _isenabled;
        private int? _prioritylevel;
        private string _spliteline;
        private string _bgimageurl;
        private string _fontcolor;
        private string _startversion;
        private string _endversion;
        private string _title;
        private string _titleimageurl;
        private string _titlecolor;
        private bool _ismore;
        private string _moreuri;
        private bool _istag;
        private string _tagcontent;
        private int? _channel;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? FKHomePageModuleID
        {
            set { _fkhomepagemoduleid = value; }
            get { return _fkhomepagemoduleid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ModuleName
        {
            set { _modulename = value; }
            get { return _modulename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ModuleType
        {
            set { _modeultype = value; }
            get { return _modeultype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set { _isenabled = value; }
            get { return _isenabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? PriorityLevel
        {
            set { _prioritylevel = value; }
            get { return _prioritylevel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SpliteLine
        {
            set { _spliteline = value; }
            get { return _spliteline; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BgImageUrl
        {
            set { _bgimageurl = value; }
            get { return _bgimageurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FontColor
        {
            set { _fontcolor = value; }
            get { return _fontcolor; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string StartVersion
        {
            set { _startversion = value; }
            get { return _startversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EndVersion
        {
            set { _endversion = value; }
            get { return _endversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TitleImageUrl
        {
            set { _titleimageurl = value; }
            get { return _titleimageurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TitleColor
        {
            set { _titlecolor = value; }
            get { return _titlecolor; }
        }


        public string TitleBgColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMore
        {
            set { _ismore = value; }
            get { return _ismore; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MoreUri
        {
            set { _moreuri = value; }
            get { return _moreuri; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsTag
        {
            set { _istag = value; }
            get { return _istag; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TagContent
        {
            set { _tagcontent = value; }
            get { return _tagcontent; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Channel
        {
            set { _channel = value; }
            get { return _channel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }

        /// <summary>
        /// 城市的PKID s
        /// </summary>
        public string City { get; set; }

        #endregion Model

        /// <summary>
        /// 动画模块的排版 1111 112 121 211 22
        /// </summary>
        public int? Pattern { get; set; }

        public int? Margin { get; set; }

        public string UriCount { get; set; }


        /// <summary>
        /// 是否是新人
        /// </summary>
        public int? IsNewUser { get; set; }

        public string NoticeChannel { get; set; }
    }

    /// <summary>
    /// 多城市附属模块对应关系表
    /// </summary>
    public class SE_ModuleHelperCityConfig
    {
        #region Model
        private int _id;
        private int? _fkhomepagemodulehelperid;
        private int? _fkregionpkid;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 附属模块的ID
        /// </summary>
        public int? FKHomePageModuleHelperID
        {
            set { _fkhomepagemodulehelperid = value; }
            get { return _fkhomepagemodulehelperid; }
        }
        /// <summary>
        /// 城市的PKID
        /// </summary>
        public int? FKRegionPKID
        {
            set { _fkregionpkid = value; }
            get { return _fkregionpkid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }

        public int? ParentID { get; set; }
        #endregion Model
    }


    /// <summary>
    /// 模块内容列表
    /// </summary>
    public class SE_HomePageModuleContentConfig
    {
        public SE_HomePageModuleContentConfig()
        {

        }

        #region Model
        private int _id;
        private int? _fkhomepagemoduleid;
        private int? _fkhomepagemodulehelperid;
        private string _startversion;
        private string _endversion;
        private string _title;
        private int? _devicetype;
        private int? _prioritylevel;
        private string _buttonimageurl;
        private string _bannerimageurl;
        private string _linkurl;
        private string _uricount;
        private bool _isenabled;
        private DateTime? _startdatetime;
        private DateTime? _enddatetime;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime;
        private int? _width;
        private int? _height;
        private int? _upperleftx;
        private int? _upperlefty;
        private int? _lowerrightx;
        private int? _lowerrighty;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? FKHomePageModuleID
        {
            set { _fkhomepagemoduleid = value; }
            get { return _fkhomepagemoduleid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? FKHomePageModuleHelperID
        {
            set { _fkhomepagemodulehelperid = value; }
            get { return _fkhomepagemodulehelperid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string StartVersion
        {
            set { _startversion = value; }
            get { return _startversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EndVersion
        {
            set { _endversion = value; }
            get { return _endversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? DeviceType
        {
            set { _devicetype = value; }
            get { return _devicetype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? PriorityLevel
        {
            set { _prioritylevel = value; }
            get { return _prioritylevel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ButtonImageUrl
        {
            set { _buttonimageurl = value; }
            get { return _buttonimageurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BannerImageUrl
        {
            set { _bannerimageurl = value; }
            get { return _bannerimageurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LinkUrl
        {
            set { _linkurl = value; }
            get { return _linkurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UriCount
        {
            set { _uricount = value; }
            get { return _uricount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set { _isenabled = value; }
            get { return _isenabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDateTime
        {
            set { _startdatetime = value; }
            get { return _startdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTime
        {
            set { _enddatetime = value; }
            get { return _enddatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Width
        {
            set { _width = value; }
            get { return _width; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Height
        {
            set { _height = value; }
            get { return _height; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? UpperLeftX
        {
            set { _upperleftx = value; }
            get { return _upperleftx; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? UpperLeftY
        {
            set { _upperlefty = value; }
            get { return _upperlefty; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? LowerRightX
        {
            set { _lowerrightx = value; }
            get { return _lowerrightx; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? LowerRightY
        {
            set { _lowerrighty = value; }
            get { return _lowerrighty; }
        }
        #endregion Model

        public string BigTitle { get; set; }

        public string BigTilteColor { get; set; }

        public string SmallTitle { get; set; }

        public string SmallTitleColor { get; set; }

        public string PromoteTitle { get; set; }

        public string PromoteTitleColor { get; set; }

        public string PromoteTitleBgColor { get; set; }

        public int AnimationStyle { get; set; }

        public string UserRank { get; set; }

        public string VIPRank { get; set; }

        /// <summary>
        /// 人群
        /// </summary>
        public string PeopleTip { get; set; }

        /// <summary>
        /// 应用市场来源
        /// </summary>
        public string NoticeChannel { get; set; }

        /// <summary>
        /// 应用市场渠道
        /// </summary>
        public string NewNoticeChannel { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string DeviceBrand { get; set; }


        /// <summary>
        /// 机型
        /// </summary>
        public string DeviceTypes { get; set; }

    }


    /// <summary>
    /// 参数对应关系实体
    /// </summary>
    public class SE_WapParameterConfig
    {
        #region Model
        private int _id;
        private string _name;
        private string _keyname;
        private string _android;
        private string _ios;
        private DateTime? _createdatetime;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string KeyName
        {
            set { _keyname = value; }
            get { return _keyname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Android
        {
            set { _android = value; }
            get { return _android; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string IOS
        {
            set { _ios = value; }
            get { return _ios; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        #endregion Model
    }

    /// <summary>
    /// 闪屏配置实体
    /// </summary>
    public class SE_FlashScreenConfig
    {
        public SE_FlashScreenConfig()
        { }
        #region Model
        private int _id;
        private string _startversion;
        private string _endversion;
        private string _name;
        private int? _prioritylevel;
        private int? _channel;
        private string _buttonimage;
        private string _bannerimage;
        private string _appurl;
        private string _h5url;
        private string _androidapphandler;
        private string _androidcommunication;
        private string _iosapphandler;
        private string _ioscommunication;
        private string _counts;
        private bool _isenabled;
        private DateTime? _startdatetime;
        private DateTime? _enddatetime;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string StartVersion
        {
            set { _startversion = value; }
            get { return _startversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EndVersion
        {
            set { _endversion = value; }
            get { return _endversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? PriorityLevel
        {
            set { _prioritylevel = value; }
            get { return _prioritylevel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Channel
        {
            set { _channel = value; }
            get { return _channel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ButtonImage
        {
            set { _buttonimage = value; }
            get { return _buttonimage; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BannerImage
        {
            set { _bannerimage = value; }
            get { return _bannerimage; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LinkUrl
        {
            get; set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Counts
        {
            set { _counts = value; }
            get { return _counts; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set { _isenabled = value; }
            get { return _isenabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDateTime
        {
            set { _startdatetime = value; }
            get { return _startdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTime
        {
            set { _enddatetime = value; }
            get { return _enddatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }
        #endregion Model

        public string Zip { get; set; }


        public string Md5 { get; set; }

        public int Type { get; set; }

        public string APPChannel { get; set; }

        public string NewNoticeChannel { get; set; }

    }

    /// <summary>
    /// 瀑布里配置
    /// </summary>
    public class SE_HomePageFlowConfig
    {
        #region Model
        private int _id;
        private string _title;
        private string _bgimageurl;
        private string _startversion;
        private string _endversion;
        private int? _channel;
        private string _linkurl;
        private string _uricount;
        private bool _isenabled;
        private bool _iscountdown;
        private DateTime? _startdatetime;
        private DateTime? _enddatetime;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime;
        private bool _ischildproduct;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BgImageUrl
        {
            set { _bgimageurl = value; }
            get { return _bgimageurl; }
        }
        public string SmallBgImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StartVersion
        {
            set { _startversion = value; }
            get { return _startversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EndVersion
        {
            set { _endversion = value; }
            get { return _endversion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Channel
        {
            set { _channel = value; }
            get { return _channel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LinkUrl
        {
            set { _linkurl = value; }
            get { return _linkurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UriCount
        {
            set { _uricount = value; }
            get { return _uricount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set { _isenabled = value; }
            get { return _isenabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCountDown
        {
            set { _iscountdown = value; }
            get { return _iscountdown; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDateTime
        {
            set { _startdatetime = value; }
            get { return _startdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTime
        {
            set { _enddatetime = value; }
            get { return _enddatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsChildProduct
        {
            set { _ischildproduct = value; }
            get { return _ischildproduct; }
        }

        public int PriorityLevel { get; set; }
        #endregion Model

        public string Type { get; set; }

        public int? ParentPKID { get; set; }

        public string BigTitle { get; set; }

        public string BigTilteColor { get; set; }

        public string SmallTitle { get; set; }

        public string SmallTitleColor { get; set; }

        public string APPChannel { get; set; }

    }


    /// <summary>
    /// 瀑布流产品
    /// </summary>
    public class SE_HomePageFlowProductConfig
    {
        #region Model
        private int _id;
        private int? _fkhomepageflowconfig;
        private string _pid;
        private string _displayname;
        private decimal? _price;
        private Guid _pkflashsale;
        private int? _prioritylevel;
        private bool _isenabled;
        private DateTime? _createdatetime;
        private DateTime? _updatedatetime;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? FKHomePageFlowConfig
        {
            set { _fkhomepageflowconfig = value; }
            get { return _fkhomepageflowconfig; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PID
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DisplayName
        {
            set { _displayname = value; }
            get { return _displayname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Price
        {
            set { _price = value; }
            get { return _price; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid? PKFlashSale
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? PriorityLevel
        {
            set { _prioritylevel = value; }
            get { return _prioritylevel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set { _isenabled = value; }
            get { return _isenabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set { _createdatetime = value; }
            get { return _createdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set { _updatedatetime = value; }
            get { return _updatedatetime; }
        }
        #endregion Model
    }


    /// <summary>
    /// 首页列表
    /// </summary>
    public class SE_HomePageMenuListConfig
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }

        /// <summary>
        /// 开始版本号
        /// </summary>
        public string StartVersion { get; set; }

        /// <summary>
        /// 结束版本号
        /// </summary>
        public string EndVersion { get; set; }


    }


    /// <summary>
    /// 首页底部菜单
    /// </summary>
    public class SE_HomePageMenuConfig
    {
        public int ID { get; set; }


        public int? FK_MenuListID { get; set; }

        public string MenuType { get; set; }

        public string MenuName { get; set; }

        public string ShowImageUrl { get; set; }

        public string ClickImageUrl { get; set; }


        public int PriorityLevel { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }

        public string ShowFontColor { get; set; }

        public string ClickFontColor { get; set; }


    }

    /// <summary>
    /// 动画内容
    /// </summary>
    public class SE_HomePageAnimationContent
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string StartVersion { get; set; }

        public string EndVersion { get; set; }

        public string BannerImageUrl { get; set; }

        public string ButtonImageUrl { get; set; }

        public string LinkUrl { get; set; }

        public string TrackingId { get; set; }

        public string BigTitle { get; set; }

        public string BigTilteColor { get; set; }

        public string SmallTitle { get; set; }

        public string SmallTitleColor { get; set; }

        public string PromoteTitle { get; set; }

        public string PromoteTitleColor { get; set; }

        public string PromoteTitleBgColor { get; set; }

        public int AnimationStyle { get; set; }

        public int PriorityLevel { get; set; }

        public bool IsEnabled { get; set; }

        public int FKModuleID { get; set; }

        public int FKModuleHelper { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }
    }

    public class DeviceBrandModel
    {
        public int Pkid { get; set; }

        public string DeviceBrand { get; set; }
    }
    public class DeviceTypeModel
    {
        public int Pkid { get; set; }

        public string DeviceType { get; set; }
    }
}
