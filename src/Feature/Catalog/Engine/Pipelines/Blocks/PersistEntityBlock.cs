//using Microsoft.Extensions.Logging;
//using Sitecore.Commerce.Core;
//using Sitecore.Commerce.Core.Caching;
//using Sitecore.Commerce.Core.Commands;
//using Sitecore.Commerce.Plugin.SQL;
//using Sitecore.Framework.Caching;
//using Sitecore.Framework.Conditions;
//using Sitecore.Framework.Pipelines;
//using System;
//using System.Threading.Tasks;

//namespace Feature.Catalog.Engine.Pipelines.Blocks
//{
//    [PipelineDisplayName("SQL:block:persistentity")]
//    public class PersistEntityBlock : ConditionalPipelineBlock<PersistEntityArgument, PersistEntityArgument, CommercePipelineExecutionContext>
//    {
//        private readonly IGetEnvironmentCachePipeline _cachePipeline;
//        private readonly InsertEntityCommand _insertEntityCommand;
//        private readonly UpdateEntityCommand _updateEntityCommand;
//        private readonly EntitySerializerCommand _entitySerializerCommand;

//        public PersistEntityBlock(
//          IGetEnvironmentCachePipeline cachePipeline,
//          InsertEntityCommand insertEntityCommand,
//          UpdateEntityCommand updateEntityCommand,
//          EntitySerializerCommand entitySerializerCommand)
//        {
//            _cachePipeline = cachePipeline;
//            _insertEntityCommand = insertEntityCommand;
//            _updateEntityCommand = updateEntityCommand;
//            _entitySerializerCommand = entitySerializerCommand;
//            BlockCondition = new Predicate<IPipelineExecutionContext>(ValidatePolicy);
//        }

//        public override Task<PersistEntityArgument> ContinueTask(
//          PersistEntityArgument arg,
//          CommercePipelineExecutionContext context)
//        {
//            return Task.FromResult(arg);
//        }

//        public override async Task<PersistEntityArgument> Run(
//          PersistEntityArgument arg,
//          CommercePipelineExecutionContext context)
//        {
//            PersistEntityBlock persistEntityBlock = this;
//            Condition.Requires(arg).IsNotNull(string.Format("{0}: the argument cannot be null.", persistEntityBlock.Name));
//            if (string.IsNullOrEmpty(arg.Entity.Id))
//                arg.Entity.Id = Guid.NewGuid().ToString("N");
//            context.Logger.LogInformation(string.Format("{0}.{1}: EntityId={2}", persistEntityBlock.Name, arg.Entity.GetType().Name, arg.Entity.Id));
//            if (arg.Entity.Version <= 0)
//            {
//                Exception exception = new Exception("Entity version cannot be lower than or equal to 0.");
//                string str = await context.CommerceContext.AddMessage(
//                    context.GetPolicy<KnownResultCodes>().Error,
//                    persistEntityBlock.Name,
//                    new object[1] { exception },
//                    string.Format("{0}.Version.Exception: {1}", persistEntityBlock.Name, exception.Message));
//                return arg;
//            }
//            bool isPersisted = arg.Entity.IsPersisted;
//            arg.Entity.IsPersisted = true;
//            try
//            {
//                PersistEntityArgument persistEntityArgument = arg;
//                persistEntityArgument.SerializedEntity = await persistEntityBlock._entitySerializerCommand.SerializeEntity(context.CommerceContext, arg.Entity);
//                persistEntityArgument = null;
//            }
//            catch (Exception ex)
//            {
//                string str = await context.CommerceContext.AddMessage(
//                    context.GetPolicy<KnownResultCodes>().Error,
//                    persistEntityBlock.Name,
//                    new object[1] { ex },
//                    string.Format("{0}.Serialize.Exception: {1}", persistEntityBlock.Name, ex.Message));
//                return arg;
//            }
//            if (!isPersisted)
//            {
//                try
//                {
//                    CommerceCommand commerceCommand = await persistEntityBlock._insertEntityCommand.Process(context.CommerceContext, arg.Entity.Id, arg.Entity.Version, arg.Entity.EntityVersion, arg.SerializedEntity, arg.Entity.Published);
//                }
//                catch (Exception ex)
//                {
//                    string str = await context.CommerceContext.AddMessage(
//                        context.GetPolicy<KnownResultCodes>().Error,
//                        persistEntityBlock.Name,
//                        new object[1] { ex },
//                        string.Format("{0}.Insert.Exception: {1}", persistEntityBlock.Name, ex.Message));
//                    return arg;
//                }
//            }
//            else
//            {
//                try
//                {
//                    CommerceCommand commerceCommand = await persistEntityBlock._updateEntityCommand.Process(context.CommerceContext, arg.Entity.Id, arg.Entity.Version, arg.Entity.EntityVersion, arg.SerializedEntity, arg.Entity.Published);
//                }
//                catch (Exception ex)
//                {
//                    string str = await context.CommerceContext.AddMessage(
//                        context.GetPolicy<KnownResultCodes>().Error,
//                        persistEntityBlock.Name,
//                        new object[1] { ex },
//                        string.Format("{0}.Update.Exception: {1}", persistEntityBlock.Name, ex.Message));
//                    return arg;
//                }
//            }
//            EntityMemoryCachingPolicy entityCachePolicy = EntityMemoryCachingPolicy.GetCachePolicy(context.CommerceContext, arg.Entity.GetType());
//            if (!entityCachePolicy.AllowCaching)
//                return arg;
//            string itemKey = string.IsNullOrEmpty(arg.Entity.CompositeKey) ? string.Format("{0}", arg.Entity.Id) : string.Format("{0}", arg.Entity.CompositeKey);
//            CacheRequest model = context.GetModel<CacheRequest>(x => x.EntityId.Equals(arg.Entity.Id, StringComparison.OrdinalIgnoreCase));
//            if (model != null && model.Version.HasValue)
//                itemKey += string.Format("-{0}", model.Version);
//            else if (model == null)
//                itemKey += string.Format("-{0}", arg.Entity.EntityVersion);
//            IGetEnvironmentCachePipeline cachePipeline = persistEntityBlock._cachePipeline;
//            EnvironmentCacheArgument environmentCacheArgument = new EnvironmentCacheArgument();
//            environmentCacheArgument.CacheName = entityCachePolicy.CacheName;
//            CommercePipelineExecutionContext context1 = context;
//            ICache cache = await cachePipeline.Run(environmentCacheArgument, context1);
//            if (entityCachePolicy.CacheAsEntity)
//                await cache.Set(itemKey, new Cachable<CommerceEntity>(arg.Entity, 1L), entityCachePolicy.GetCacheEntryOptions());
//            else
//                await cache.Set(itemKey, new Cachable<string>(arg.SerializedEntity, 1L), entityCachePolicy.GetCacheEntryOptions());
//            return arg;
//        }

//        private bool ValidatePolicy(IPipelineExecutionContext obj)
//        {
//            return ((CommercePipelineExecutionContext)obj).CommerceContext.HasPolicy<EntityStoreSqlPolicy>();
//        }
//    }
//}
