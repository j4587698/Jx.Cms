﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
 using Jx.Cms.Common.Utils;
 using Jx.Cms.Plugin;
using Jx.Cms.Themes.FileProvider;
 using Jx.Cms.Themes.Util;
 using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Themes.Options
{
    public class UiConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        public UiConfigureOptions(IWebHostEnvironment environment)
        {
            Environment = environment;
            ThemeUtil.ThemeModify = ChangeTheme;
            ThemeUtil.InitThemePath();
        }

        private readonly MyCompositeFileProvider _filesProvider = new();
        private string basePath = "wwwroot";

        private void ChangeTheme(ThemeConfig themeConfig)
        {
            var assembly = RazorPlugin.GetAssemblyByThemeType(themeConfig.ThemeType);
            if (assembly != null)
            {
                _filesProvider.ModifyFileProvider(new EmbeddedFileProvider(assembly, $"{Path.GetFileNameWithoutExtension(themeConfig.Path)}.{basePath}"), themeConfig.ThemeType);
            }
        }

        private IWebHostEnvironment Environment { get; }
        
        private void AddPhysicalFileProvider(List<IFileProvider> providers, IFileProvider fileProvider)
        {
            if (fileProvider == null) return;
            
            Console.WriteLine($"Processing WebRootFileProvider: {fileProvider.GetType().FullName}");
            
            if (fileProvider is PhysicalFileProvider)
            {
                providers.Add(fileProvider);
                Console.WriteLine($"Added PhysicalFileProvider: {fileProvider.GetType().FullName}");
            }
            else if (fileProvider is CompositeFileProvider composite)
            {
                // 展开 CompositeFileProvider 查找 PhysicalFileProvider
                var field = typeof(CompositeFileProvider).GetField("_fileProviders", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field?.GetValue(composite) is IEnumerable<IFileProvider> innerProviders)
                {
                    Console.WriteLine($"  CompositeFileProvider contains {innerProviders.Count()} providers:");
                    bool foundPhysicalProvider = false;
                    
                    foreach (var provider in innerProviders)
                    {
                        Console.WriteLine($"    - {provider?.GetType().FullName ?? "null"}");
                        
                        if (provider is PhysicalFileProvider)
                        {
                            providers.Add(provider);
                            Console.WriteLine($"Added PhysicalFileProvider from Composite: {provider.GetType().FullName}");
                            foundPhysicalProvider = true;
                        }
                        else if (provider is CompositeFileProvider innerComposite)
                        {
                            // 递归展开内层的 CompositeFileProvider
                            Console.WriteLine($"    Recursively processing inner CompositeFileProvider");
                            AddPhysicalFileProvider(providers, innerComposite);
                            foundPhysicalProvider = true;
                        }
                        else if (provider != null)
                        {
                            // 添加其他非 null 的提供程序
                            providers.Add(provider);
                            Console.WriteLine($"Added provider from Composite: {provider.GetType().FullName}");
                        }
                    }
                    
                    if (foundPhysicalProvider)
                    {
                        return;
                    }
                }
                
                // 如果没有找到 PhysicalFileProvider，添加整个 CompositeFileProvider
                providers.Add(composite);
                Console.WriteLine($"Added CompositeFileProvider (no PhysicalFileProvider found): {composite.GetType().FullName}");
            }
            else
            {
                // 其他类型的 FileProvider，直接添加
                providers.Add(fileProvider);
                Console.WriteLine($"Added FileProvider: {fileProvider.GetType().FullName}");
            }
        }
        
        private bool IsPhysicalFileProviderOrContainsPhysicalFileProvider(IFileProvider provider, IFileProvider webRootProvider)
        {
            if (provider == webRootProvider) return true;
            
            if (provider is PhysicalFileProvider) return true;
            
            if (provider is CompositeFileProvider composite)
            {
                var field = typeof(CompositeFileProvider).GetField("_fileProviders", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field?.GetValue(composite) is IEnumerable<IFileProvider> innerProviders)
                {
                    foreach (var innerProvider in innerProviders)
                    {
                        if (innerProvider == webRootProvider || innerProvider is PhysicalFileProvider)
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

        public void PostConfigure(string name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();
            if (options.FileProvider == null && Environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider ??= Environment.WebRootFileProvider;
            
            // 确保文件提供程序不为 null
            if (options.FileProvider == null)
            {
                options.FileProvider = Environment.WebRootFileProvider ?? new PhysicalFileProvider(Environment.WebRootPath);
            }
            
            // 创建一个包含所有文件提供程序的列表
            var providers = new List<IFileProvider>();
            
            // 首先确保 PhysicalFileProvider 存在（用于 wwwroot 静态文件）
            // 获取实际的 PhysicalFileProvider，如果 WebRootFileProvider 是 CompositeFileProvider，则展开它
            AddPhysicalFileProvider(providers, Environment.WebRootFileProvider);
            
            // 添加现有文件提供程序
            if (options.FileProvider != null && options.FileProvider != Environment.WebRootFileProvider)
            {
                Console.WriteLine($"Existing FileProvider: {options.FileProvider.GetType().FullName}");
                
                // 如果现有文件提供程序已经是 CompositeFileProvider，展开它
                if (options.FileProvider is CompositeFileProvider existingComposite)
                {
                    Console.WriteLine("Unwrapping existing CompositeFileProvider");
                    
                    // 通过反射获取私有字段 _fileProviders
                    var field = typeof(CompositeFileProvider).GetField("_fileProviders", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (field?.GetValue(existingComposite) is IEnumerable<IFileProvider> innerProviders)
                    {
                        foreach (var provider in innerProviders)
                        {
                            if (provider != null)
                            {
                                // 检查是否是 PhysicalFileProvider 或者是包含 PhysicalFileProvider 的 CompositeFileProvider
                                if (!IsPhysicalFileProviderOrContainsPhysicalFileProvider(provider, Environment.WebRootFileProvider))
                                {
                                    providers.Add(provider);
                                    Console.WriteLine($"  Added inner provider: {provider.GetType().FullName}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("  Found null inner provider!");
                            }
                        }
                    }
                    else
                    {
                        // 如果无法获取内部提供程序，将整个复合提供程序作为一个提供程序
                        providers.Add(existingComposite);
                        Console.WriteLine("Could not unwrap CompositeFileProvider, adding as single provider");
                    }
                }
                else
                {
                    // 不是复合提供程序，直接添加
                    providers.Add(options.FileProvider);
                }
            }
            
            // 添加额外文件提供程序
            providers.Add(_filesProvider);
            Console.WriteLine($"Additional FileProvider: {_filesProvider.GetType().FullName}");
            
            // 检查是否有 null 的文件提供程序
            for (int i = 0; i < providers.Count; i++)
            {
                if (providers[i] == null)
                {
                    Console.WriteLine($"Provider at index {i} is null!");
                }
            }
            
            // 创建复合文件提供程序
            options.FileProvider = new CompositeFileProvider(providers);
            Console.WriteLine($"Created CompositeFileProvider with {providers.Count} providers");

            Common.Utils.Util.ThemeProvider = options.FileProvider;
        }
    }
}