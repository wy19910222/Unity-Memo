/*
 * @Author: wangyun
 * @CreateTime: 2024-07-01 00:16:28 640
 * @LastEditor: wangyun
 * @EditTime: 2024-07-01 00:16:28 644
 */

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Memo006_GetFieldPath.Editor {
	public static class FieldPathUtils {
		public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr) {
			List<string> member = new List<string>();
			if (expr.Body.NodeType == ExpressionType.MemberAccess) {
				MemberExpression me = expr.Body as MemberExpression;
				while (me != null) {
					member.Add(me.Member.Name);
					me = me.Expression as MemberExpression;
				}
				member.Reverse();
			}
			return string.Join(".", member);
		}
	}
}