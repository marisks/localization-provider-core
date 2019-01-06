using System.Collections.Generic;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.AspNetCore.Queries;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.Core.PerfTests
{
    public class DbStringLocalizerPerfTests
    {
        [GlobalSetup]
        public void GlobalSetup()
        {
            ConfigurationContext.Current.CacheManager = new InMemoryCache();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslationHandler>();

            for(var i = 0; i < 5000; i++)
            {
                ConfigurationContext.Current.CacheManager.Insert(CacheKeyHelper.BuildKey($"SampleResourceKey__{i}"),
                                                                 new LocalizationResource
                                                                 {
                                                                     Id = 1,
                                                                     ResourceKey = $"SampleResourceKey__{i}",
                                                                     Translations = new List<LocalizationResourceTranslation>
                                                                                    {
                                                                                        new LocalizationResourceTranslation
                                                                                        {
                                                                                            Language = "en",
                                                                                            Value = $"English Translation {i}"
                                                                                        }
                                                                                    }
                                                                 });

                if(i == 4000)
                {
                    // insert our test resource somewhere in the middle of the list

                    ConfigurationContext.Current.CacheManager.Insert(CacheKeyHelper.BuildKey("SampleResourceKey"),
                                                                     new LocalizationResource
                                                                     {
                                                                         Id = 1,
                                                                         ResourceKey = "SampleResourceKey",
                                                                         Translations = new List<LocalizationResourceTranslation>
                                                                                        {
                                                                                            new LocalizationResourceTranslation
                                                                                            {
                                                                                                Language = "en",
                                                                                                Value = "English Translation"
                                                                                            }
                                                                                        }
                                                                     });
                }
            }
        }

        [Benchmark]
        public string GetTranslation()
        {
            var sut = new DbStringLocalizer(new CultureInfo("en")) as IStringLocalizer;
            return sut["SampleResourceKey"];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DbStringLocalizerPerfTests>();
        }
    }
}
