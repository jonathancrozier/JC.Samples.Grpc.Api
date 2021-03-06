﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JC.Samples.Grpc.Api
{
    /// <summary>
    /// Exposes Todo endpoints.
    /// </summary>
    public class TodoService : Todos.TodosBase
    {
        #region Fields

        /// <summary>
        /// Holds an in-memory list of Todo items for simulation purposes.
        /// </summary>
        private static ICollection<Todo> _todos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Buy milk", UserId = 1 },
                new Todo { Id = 2, Title = "Leave out the trash", UserId = 2 },
                new Todo { Id = 3, Title = "Clean room", UserId = 2 }
            };

        #endregion

        #region Readonlys

        private readonly ILogger<TodoService> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public TodoService(ILogger<TodoService> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes all Todos.
        /// </summary>
        /// <param name="request">The Empty Request message</param>
        /// <param name="context">The context for the RPC call</param>
        /// <returns>An Empty Response</returns>
        [Authorize]
        public override Task<Empty> DeleteTodos(Empty request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            
            foreach (var claim in user.Claims)
            {
                Trace.WriteLine(claim);
            }

            _todos.Clear();

            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// Gets a collection of Todos.
        /// </summary>
        /// <param name="request">The Todo Request message</param>
        /// <param name="context">The context for the RPC call</param>
        /// <returns>A Todo Response containing a collection of available Todos</returns>
        public override Task<TodoResponse> GetTodos(TodoRequest request, ServerCallContext context)
        {
            var response = new TodoResponse();
            response.Todos.AddRange(_todos.Where(t => t.UserId == request.UserId));

            return Task.FromResult(response);
        }

        #endregion
    }
}