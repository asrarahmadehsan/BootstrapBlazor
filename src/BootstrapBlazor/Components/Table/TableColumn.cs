﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 表头组件
    /// </summary>
    public class TableColumn<TItem> : BootstrapComponentBase, ITableColumn
    {
        /// <summary>
        /// 获得/设置 绑定列类型
        /// </summary>
        public Type? FieldType { get; set; }

#nullable disable
        /// <summary>
        /// 获得/设置 数据绑定字段值
        /// </summary>
        [Parameter]
        public TItem Field { get; set; }
#nullable restore

        /// <summary>
        /// 获得/设置 ValueExpression 表达式
        /// </summary>
        [Parameter]
        public Expression<Func<TItem>>? FieldExpression { get; set; }

        /// <summary>
        /// 获得/设置 是否排序 默认 false
        /// </summary>
        [Parameter]
        public bool Sortable { get; set; }

        /// <summary>
        /// 获得/设置 是否可过滤数据 默认 false
        /// </summary>
        [Parameter]
        public bool Filterable { get; set; }

        /// <summary>
        /// 获得/设置 表头显示文字
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// 获得/设置 列宽 默认为 auto
        /// </summary>
        [Parameter]
        public int Width { get; set; }

        /// <summary>
        /// 获得/设置 模板
        /// </summary>
        [Parameter]
        public RenderFragment<TableColumnContext<object, TItem>>? Template { get; set; }

        /// <summary>
        /// 内部使用负责把 object 类型的绑定数据值转化为泛型数据传递给前端
        /// </summary>
        RenderFragment<object>? ITableColumn.Template
        {
            get => this.Template == null ? null : new RenderFragment<object>(context => builder =>
            {
                // 此处 context 为行数据
                // 将绑定字段值放入上下文中
                var invoker = GetPropertyCache.GetOrAdd(GetFieldName(), key => context.GetPropertyValueLambda<object, TItem>(key).Compile());
                var value = invoker(context);
                builder.AddContent(0, this.Template.Invoke(new TableColumnContext<object, TItem>() { Row = context, Value = value }));
            });
        }

        /// <summary>
        /// 获得/设置 Table Header 实例
        /// </summary>
        [CascadingParameter]
        protected TableColumnCollection? Columns { get; set; }

        /// <summary>
        /// 组件初始化方法
        /// </summary>
        protected override void OnInitialized()
        {
            Columns?.Columns.Add(this);
            _fieldIdentifier = FieldIdentifier.Create(FieldExpression);

            // 获取模型属性定义类型
            FieldType = _fieldIdentifier.Value.Model.GetType().GetProperty(GetFieldName())?.PropertyType;
        }

        private FieldIdentifier? _fieldIdentifier;
        /// <summary>
        /// 获取绑定字段显示名称方法
        /// </summary>
        public string GetDisplayName() => Text ?? _fieldIdentifier?.GetDisplayName() ?? "";

        /// <summary>
        /// 获取绑定字段信息方法
        /// </summary>
        public string GetFieldName() => _fieldIdentifier?.FieldName ?? "";

        private static readonly ConcurrentDictionary<string, Func<object, TItem>> GetPropertyCache = new ConcurrentDictionary<string, Func<object, TItem>>();
    }
}
