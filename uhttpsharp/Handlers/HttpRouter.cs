/*
 * Copyright (C) 2011 uhttpsharp project - http://github.com/raistlinthewiz/uhttpsharp
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.

 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.

 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uhttpsharp.Handlers
{
    public class HttpRouter : IHttpRequestHandler
    {
        private readonly IDictionary<string, IHttpRequestHandler> _handlers = new Dictionary<string, IHttpRequestHandler>(StringComparer.InvariantCultureIgnoreCase);

        private readonly static string StateLevelPropertyName = "HttpRouteLevel";

        public HttpRouter With(string function, IHttpRequestHandler handler)
        {
            _handlers.Add(function, handler);

            return this;
        }

        public Task Handle(IHttpContext context, Func<Task> nextHandler)
        {
            string function = string.Empty;

            int index = GetRouteLevel(context.State);

            if (context.Request.RequestParameters.Length > 0 && index < context.Request.RequestParameters.Length)
            {
                function = context.Request.RequestParameters[index];
                if (!string.IsNullOrEmpty(function))
                {
                    //we are handling this path
                    IncreaseRouteLevel(context.State);
                }
            }

            IHttpRequestHandler value;
            if (_handlers.TryGetValue(function, out value))
            {
                return value.Handle(context, nextHandler);
            }
            

            // Route not found, Call next.
            return nextHandler();
        }

        private int GetRouteLevel(dynamic state)
        {
            if (!((IDictionary<String, object>)state).ContainsKey(StateLevelPropertyName))
            {
                state.HttpRouteLevel = 0;
            }
            return state.HttpRouteLevel;
        }

        private void IncreaseRouteLevel(dynamic state)
        {
            state.HttpRouteLevel = state.HttpRouteLevel + 1;
        }
    }
}
