//
//  Cross Platform Replay Kit
//
//  Created by Ayyappa Reddy on 03/06/19.
//  Copyright (c) 2019 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <AVKit/AVKit.h>
#import "CrossPlatformReplayKitUIOverlayWindow.h"

@protocol IReplayKitListener <NSObject>

-(void) onReplayKitRecordingStarted:(BOOL) status;
-(void) onReplayKitRecordingStopped:(BOOL) status;
-(void) onReplayKitRecordingVideoAvailable:(BOOL) status;
-(void) onReplayKitRecordingVideoFailed:(const char*) error;
-(void) onReplayKitRecordingUiStartAction:(const char*) error;
-(void) onReplayKitRecordingUiStopAction:(const char*) error;
-(void) onReplayKitSaveToGalleryFinished: (const char*) error;

@end

typedef NS_ENUM(NSInteger, RecordingState)
{
    None,
    ReadyForRecording,
    StartedRecording
};

@interface CrossPlatformReplayKitHandler : NSObject<AVCaptureAudioDataOutputSampleBufferDelegate, IReplayKitListener>

+ (id)sharedInstance;
- (BOOL) isAPIAvailable;
- (NSString*) getPreviewFilePath;
- (BOOL) isPreviewAvailable;
- (BOOL) isCurrentlyRecording;
- (void) prepareRecording;
- (void) startRecording;
- (void) stopRecording;
- (BOOL) previewRecording;
- (BOOL) discardRecording;
- (void) setRecordingUIVisibility:(BOOL)visible;
- (void) setMicrophoneStatus:(BOOL)enable;

-(void) savePreview:(NSString*) path;
-(void) sharePreview;

@property BOOL microphoneEnabled;
@property BOOL initialisedWriter;

@property BOOL videoDataStarted;
@property BOOL audioDataStarted;
@property BOOL micDataStarted;
@property(nonatomic) RecordingState recordingState;

@property (atomic, retain) AVAssetWriter         *writer;
@property (atomic, retain) AVAssetWriterInput    *video;
@property (atomic, retain) AVAssetWriterInput    *audio;
@property (atomic, retain) AVAssetWriterInput    *mic;

@property NSString* recordingPath;

@property (atomic, retain) AVCaptureSession *session;
@property (atomic, retain) AVCaptureAudioDataOutput *micCaptureOutput;
@property (atomic, retain) AVCaptureDeviceInput *micCaptureInput;

@property(nonatomic, retain)    AVPlayerViewController        *moviePlayerVC;
@property (nonatomic)           dispatch_queue_t              sessionQueue;

@property (nonatomic, retain) CrossPlatformReplayKitUIOverlayWindow *overlayWindow;
@property (nonatomic, retain) UIButton* uiButton;

@property (nonatomic, weak) UIWindow *cachedMainWindow;

@property (nonatomic, weak) id<IReplayKitListener> listener;
@end
