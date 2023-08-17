//
//  Cross Platform Replay Kit
//
//  Created by Ayyappa Reddy on 03/06/19.
//  Copyright (c) 2019 Voxel Busters Interactive LLP. All rights reserved.
//


#import "CrossPlatformReplayKitBinding.h"
#import "CrossPlatformReplayKitHandler.h"

#pragma mark - Helpers

NSString* replaykit_convertToObjCString (const char * charStr)
{
    if (charStr == NULL)
        return NULL;
    else
        return [NSString stringWithUTF8String:charStr];
}

const char* replaykit_copyString(const char* charStr)
{
    if (charStr == NULL)
        return NULL;
    
    char* value = (char*)malloc(strlen(charStr) + 1);
    strcpy(value, charStr);
    
    return value;
}


#pragma mark - Query

BOOL replaykit_isAPIAvailable ()
{
    return [[CrossPlatformReplayKitHandler sharedInstance] isAPIAvailable];
}

BOOL replaykit_isRecording ()
{
    return [[CrossPlatformReplayKitHandler sharedInstance] isCurrentlyRecording];
}

BOOL replaykit_isPreviewAvailable ()
{
    return [[CrossPlatformReplayKitHandler sharedInstance] isPreviewAvailable];
}

const char* replaykit_getPreviewFilePath()
{
    NSString *path = [[CrossPlatformReplayKitHandler sharedInstance] getPreviewFilePath];
    return replaykit_copyString([path UTF8String]);
}

#pragma mark - Recording

void replaykit_setRecordingUIVisibility(bool visible)
{
    [[CrossPlatformReplayKitHandler sharedInstance] setRecordingUIVisibility:visible];

}

void replaykit_setMicrophoneStatus(bool enable)
{
    [[CrossPlatformReplayKitHandler sharedInstance] setMicrophoneStatus:enable];

}

void replaykit_prepareRecording()
{
    [[CrossPlatformReplayKitHandler sharedInstance] prepareRecording];
}

void replaykit_startRecording ()
{
 [[CrossPlatformReplayKitHandler sharedInstance] startRecording];
} 

void replaykit_stopRecording ()
{
 [[CrossPlatformReplayKitHandler sharedInstance] stopRecording];
}  

BOOL replaykit_previewRecording()
{
    return [[CrossPlatformReplayKitHandler sharedInstance] previewRecording];
}

BOOL replaykit_discardRecording()
{
    return [[CrossPlatformReplayKitHandler sharedInstance] discardRecording];
}

#pragma mark - Utilities

void replaykit_savePreview(const char* path)
{
    [[CrossPlatformReplayKitHandler sharedInstance] savePreview: replaykit_convertToObjCString(path)];	
}

void replaykit_sharePreview(const char* text, const char* subject)
{
    [[CrossPlatformReplayKitHandler sharedInstance] sharePreview];		
}


