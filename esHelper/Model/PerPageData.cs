using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esHelper.Common
{
    /// <summary>
    /// 分页数据
    /// </summary>
    public class PerPageData
    {
        public PerPageData()
        {

        }

        /// <summary>
        /// 通过三个参数获取得到其他分页数据
        /// </summary>
        /// <param name="pIndex">当前页</param>
        /// <param name="tRecordCount">总记录数</param>
        /// <param name="pSize">每页显示记录数</param>
        public PerPageData(int pIndex, int tRecordCount, int pSize)
        {
            navPageCount = 10;
            pageIndex = pIndex;
            totalRecordCount = tRecordCount;
            pageSize = pSize;
            totalPageCount = (int)((pageSize + totalRecordCount - 1) / pageSize);
            navBeginIndex = ((int)((pageIndex - 1) / navPageCount)) * navPageCount + 1;
            navEndIndex = navBeginIndex + navPageCount - 1;
            navPrePageIndex = navBeginIndex - 1 <= 0 ? 1 : navBeginIndex - 1;
            navNextPageIndex = navEndIndex + 1 > totalPageCount ? totalPageCount : navEndIndex + 1;
        }

        public object pageData { get; set; }

        /// <summary>
        /// 第几页
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 总数据数量
        /// </summary>
        public int totalRecordCount { get; set; }
        /// <summary>
        /// 每页显示数据量
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 底部或者顶部导航显示页码的数量
        /// </summary>
        public int navPageCount { get; set; }
        /// <summary>
        /// 总页码
        /// </summary>
        public int totalPageCount { get; set; }
        /// <summary>
        /// 当前底部或者顶部导航页码开始页
        /// </summary>
        public int navBeginIndex { get; set; }
        /// <summary>
        /// 当前底部或者顶部导航页码结束页
        /// </summary>
        public int navEndIndex { get; set; }
        /// <summary>
        /// 当前底部或者顶部导航页码上翻页
        /// </summary>
        public int navPrePageIndex { get; set; }
        /// <summary>
        /// 当前底部或者顶部导航页码下翻页
        /// </summary>
        public int navNextPageIndex { get; set; }
    }
}
