// using Volo.Abp.Modularity;
// using Volo.Abp.VirtualFileSystem;
//
// namespace Theme;
//
// [DependsOn(typeof(AbpVirtualFileSystemModule))]
// public class ThemeModule : AbpModule
// {
//     public override void ConfigureServices(ServiceConfigurationContext context)
//     {
//         Configure<AbpVirtualFileSystemOptions>(opt =>
//         {
//             opt.FileSets.AddEmbedded<ThemeModule>("Theme");
//         });
//     }
// }
