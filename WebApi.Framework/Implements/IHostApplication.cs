using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Framework
{
    public interface IHostApplication<TContext>
    {
        TContext CreateContext();
        Task ProcessRequestAsync(TContext context);
        void DisposeContext(TContext context, Exception exception);
    }
}
