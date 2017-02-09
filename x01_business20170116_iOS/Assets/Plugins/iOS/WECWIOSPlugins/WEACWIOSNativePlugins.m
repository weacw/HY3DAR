//
//  NSObject+WEACWIOSNativePlugins.m
//  WEACWIOSPlugins
//
//  Created by NSWell on 16/11/27.
//  Copyright © 2016年 NSWell. All rights reserved.
//

#import "WEACWIOSNativePlugins.h"

@implementation WEACWIOSNativePlugins
-(void)Unzip:(NSString*)path outPath:(NSString*)mOutPath
{
    ZipArchive* mZipFile =[[ZipArchive alloc]init];
    [mZipFile UnzipOpenFile:path];
    [mZipFile UnzipFileTo:mOutPath overWrite:YES];
    [mZipFile UnzipCloseFile];
}
@end




#if defined(__cplusplus)
extern "C"{
#endif
    static WEACWIOSNativePlugins* weacwIOSPlugins;
    static WEACWIOSToast* weacwIOSToast;
    static AssetslibOperat* assetslibOperat;
    static MediaTranscodes* meidaTranscodes;
    NSString* _CreateNSString(const char* string)
    {
        if(string)
            return [NSString stringWithUTF8String:string];
        return [NSString stringWithUTF8String:""];
    }
    
    
    char* _MakeStringCopy(const char* string)
    {
        if(NULL==string)return NULL;
        char* res = (char*)malloc(strlen(string)+1);
        strcpy(res, string);
        return res;
    }

    void _UnzipForUnity(const char* filePath,const char* outPath)
    {
        if(!weacwIOSPlugins)
        {
           weacwIOSPlugins=[[WEACWIOSNativePlugins alloc]init];
        }
        NSString* NSFilePath = _CreateNSString(filePath);
        NSString* NSOutPath=_CreateNSString(outPath);
        [weacwIOSPlugins Unzip:NSFilePath outPath:NSOutPath];
    }
    void _ShowIOSToast(const char* msg)
    {
        if(!weacwIOSToast)
            weacwIOSToast=[[WEACWIOSToast alloc]init];
        NSString* nsMsg = _CreateNSString(msg);
        [weacwIOSToast ShowTextTipsDialog:nsMsg toView:NULL];
    }
    void _ShowIOSAlertDialog(const char* title,const char* contentMsg,const char* buttonName)
    {
        if(!weacwIOSToast)
            weacwIOSToast=[[WEACWIOSToast alloc]init];
        NSLog(@"NSLOG-OK");
        [weacwIOSToast ShowAlertDialog:@"连接错误" content:@"网络不给力，请检查网络设置。" btnName:@"忽略"];
    }
    void _SaveVideoToAblum(const char* path)
    {
        if(!assetslibOperat)
            assetslibOperat = [[AssetslibOperat alloc]init];
        NSString* msg = _CreateNSString(path);
        [assetslibOperat SaveVideoToAblum:msg];
    }
    void _SavePhotoToAblum(char* path)
    {
        NSString *strReadAddr = _CreateNSString(path);
        UIImage *img = [UIImage imageWithContentsOfFile:strReadAddr];
        NSLog([NSString stringWithFormat:@"w:%f, h:%f", img.size.width, img.size.height]);
        if(!assetslibOperat)
            assetslibOperat=[[AssetslibOperat alloc]init];
        UIImageWriteToSavedPhotosAlbum(img, assetslibOperat,
                                       @selector(imageSaved:didFinishSavingWithError:contextInfo:), nil);
    }
    void _ReEncoding(char *fileInput,char *fileOut)
    {
        if(!meidaTranscodes)
            meidaTranscodes=[[MediaTranscodes alloc]init];
        NSString *input = _CreateNSString(fileInput);
        NSString *output = _CreateNSString(fileOut);
        [meidaTranscodes TranscodesLibraryVideo:input toURL:output];
    }
#if defined(__cplusplus)
}
#endif
