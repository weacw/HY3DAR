//
//  MediaTranscodes.m
//  MediaTranscode
//
//  Created by NSWell on 17/1/20.
//  Copyright © 2017年 NSWell. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MediaTranscodes.h"
@implementation MediaTranscodes
-(void)TranscodesLibraryVideo:(NSString *)path toURL:(NSString *)fileURL
{
    AVURLAsset * videoAsset = [AVURLAsset URLAssetWithURL:[NSURL fileURLWithPath:path] options:nil];
    
    AVAssetTrack *sourceVideoTrack = [[videoAsset tracksWithMediaType:AVMediaTypeVideo] objectAtIndex:0];
    AVAssetTrack *sourceAudioTrack = [[videoAsset tracksWithMediaType:AVMediaTypeAudio] objectAtIndex:0];
    
    AVMutableComposition* composition = [AVMutableComposition composition];
    
    AVMutableCompositionTrack *compositionVideoTrack = [composition addMutableTrackWithMediaType:AVMediaTypeVideo preferredTrackID:kCMPersistentTrackID_Invalid];
    [compositionVideoTrack insertTimeRange:CMTimeRangeMake(kCMTimeZero, videoAsset.duration)
                                   ofTrack:sourceVideoTrack
                                    atTime:kCMTimeZero error:nil];
    [compositionVideoTrack setPreferredTransform:videoAsset.preferredTransform];
    
    AVMutableCompositionTrack *compositionAudioTrack = [composition addMutableTrackWithMediaType:AVMediaTypeAudio
                                                                                preferredTrackID:kCMPersistentTrackID_Invalid];
    [compositionAudioTrack insertTimeRange:CMTimeRangeMake(kCMTimeZero, videoAsset.duration)
                                   ofTrack:sourceAudioTrack
                                    atTime:kCMTimeZero error:nil];

    
    CGAffineTransform finalTransform = CGAffineTransformIdentity;
    finalTransform = CGAffineTransformMakeScale(-1.0f, 1.0f);
    
    
    CGAffineTransform rotation = CGAffineTransformMakeRotation(3.14);
    CGAffineTransform translateToCenter = CGAffineTransformMakeTranslation(0, videoAsset.naturalSize.height);
    CGAffineTransform mixedTransform = CGAffineTransformConcat(rotation, translateToCenter);
    mixedTransform= CGAffineTransformConcat(mixedTransform, finalTransform);
    
    
    AVMutableVideoCompositionInstruction *instruction = [AVMutableVideoCompositionInstruction videoCompositionInstruction];
    AVMutableVideoCompositionLayerInstruction *layerInstruction = [AVMutableVideoCompositionLayerInstruction videoCompositionLayerInstructionWithAssetTrack:compositionVideoTrack];
    [layerInstruction setTransform:mixedTransform atTime:kCMTimeZero];
    
    AVMutableVideoComposition *videoComposition = [AVMutableVideoComposition videoComposition];
    videoComposition.frameDuration = CMTimeMake( 1, 30);;
    videoComposition.renderScale = 1.0;
    videoComposition.renderSize =  CGSizeMake(videoAsset.naturalSize.width, videoAsset.naturalSize.height);
    instruction.layerInstructions = [NSArray arrayWithObject: layerInstruction];
    instruction.timeRange = CMTimeRangeMake(kCMTimeZero, videoAsset.duration);
    videoComposition.instructions = [NSArray arrayWithObject: instruction];
  
    

        AVAssetExportSession * assetExport = [[AVAssetExportSession alloc] initWithAsset:composition
                                                                              presetName:AVAssetExportPresetMediumQuality];
        NSString *exportPath =[NSString stringWithFormat:fileURL];
        
        if ([[NSFileManager defaultManager] fileExistsAtPath:exportPath])
        {
            [[NSFileManager defaultManager] removeItemAtPath:exportPath error:nil];
        }
        
        
        
        
        NSLog(@"%@",exportPath);
        
        
        assetExport.outputFileType = AVFileTypeMPEG4;
        assetExport.outputURL=[NSURL fileURLWithPath:exportPath];
        
        assetExport.shouldOptimizeForNetworkUse = YES;
        assetExport.videoComposition = videoComposition;
        
        [assetExport exportAsynchronouslyWithCompletionHandler:
         ^(void ) {
             switch (assetExport.status)
             {
                 case AVAssetExportSessionStatusCompleted:
                     //                export complete
                     UnitySendMessage("Core", "UploadVideoToServer", "");
                     
                     NSLog(@"Export Complete");
                     break;
                 case AVAssetExportSessionStatusFailed:
                     NSLog(@"Export Failed");
                     NSLog(@"ExportSessionError: %@", [assetExport.error localizedDescription]);
                     //                export error (see exportSession.error)
                     break;
                 case AVAssetExportSessionStatusCancelled:
                     NSLog(@"Export Failed");
                     NSLog(@"ExportSessionError: %@", [assetExport.error localizedDescription]);
                     //                export cancelled
                     break;
             }
         }];
    }
-(CGFloat)degreesToRadians:(CGFloat)degress
{
    return (degress * 3.14)/180;
}

@end


//#if defined(__cplusplus)
//extern "C"{
//#endif
//    static MediaTranscodes* mediaTranscodes;
//    static AssetslibOperat* assetslibOperat;
//    NSString* _CreateNSString(const char* string)
//    {
//        if(string)
//            return [NSString stringWithUTF8String:string];
//        return [NSString stringWithUTF8String:""];
//    }
//    
//    
//    char* _MakeStringCopy(const char* string)
//    {
//        if(NULL==string)return NULL;
//        char* res = (char*)malloc(strlen(string)+1);
//        strcpy(res, string);
//        return res;
//    }
//    
//    void _StartTranscodes(char* filePath,char* outPath)
//    {
//        if(!mediaTranscodes)
//            mediaTranscodes =[[MediaTranscodes alloc]init];
//        NSString *file =_CreateNSString(filePath);
//        NSString *outFile=_CreateNSString(outPath);
//        [mediaTranscodes TranscodesLibraryVideo:file toURL:outFile];
//    }
//    void _SaveVideoToAblum(const char* path)
//    {
//        if(!assetslibOperat)
//            assetslibOperat = [[AssetslibOperat alloc]init];
//        NSString* msg = _CreateNSString(path);
//        [assetslibOperat SaveVideoToAblum:msg];
//    }
//
//  
//
//    
//    
//#if defined(__cplusplus)
//}
//#endif
