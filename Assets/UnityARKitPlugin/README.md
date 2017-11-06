# Unity-ARKit-Plugin #


This is a native plugin that enables using all the functionality of the ARKit SDK simply within your Unity projects for
iOS.  The plugin exposes ARKit SDK's world tracking capabilities, rendering the camera video input, plane detection and
update, point cloud extraction, light estimation, and hit testing API to Unity developers for their AR projects. This plugin is a preview quality build that
should help to get you up and running quickly with this technology, but the implementation and APIs may change to cater
to the underlying technology.

The plugin is open sourced and is released under the MIT license (see LICENSE file in this folder)

## Requirements: ##
* Unity v5.6.1p1+
* iOS 11+
* Xcode beta 9 with latest iOS SDK that contains ARKit Framework
* iOS device that supports ARKit (iPhone 6S or later, iPad (2017) or later)


## How to use this code drop:##

The code drop is a Unity project that you can load up in any Unity version that is later than v5.6.1p1.  The Unity
project contains the plugin sources and some example scenes and components that you may use in your own projects.  

Here is a summary of the important files in the plugin:

"/Assets/Plugins/iOS/UnityARKit/NativeInterface/ARSessionNative.mm" - this is the Objective-C code that actually interfaces with ARKit SDK


"/Assets/Plugins/iOS/UnityARKit/NativeInterface/UnityARSessionNativeInterface.cs" - this the scripting API to the ARKit, and provides the glue to the native code

This contains the following APIs:
	    

```
#!C#

	    public void RunWithConfigAndOptions(ARKitWorldTackingSessionConfiguration config, UnityARSessionRunOption runOptions)

	    public void RunWithConfig(ARKitWorldTackingSessionConfiguration config)

	    public void Pause()

	    public List<ARHitTestResult> HitTest(ARPoint point, ARHitTestResultType types)

	    public ARTextureHandles GetARVideoTextureHandles()

	    public float GetARAmbientIntensity()

	    public int GetARTrackingQuality()  
```


  
It also contains events that you can provide these delegates for: 


```
#!C#

        public delegate void ARFrameUpdate(UnityARCamera camera)

    	public delegate void ARAnchorAdded(ARPlaneAnchor anchorData)

        public delegate void ARAnchorUpdated(ARPlaneAnchor anchorData)

        public delegate void ARAnchorRemoved(ARPlaneAnchor anchorData)

        public delegate void ARSessionFailed(string error)
```

These are the list of events you can subscribe to:
```
#!C#
		public static event ARFrameUpdate ARFrameUpdatedEvent;
        public static event ARAnchorAdded ARAnchorAddedEvent;
        public static event ARAnchorUpdated ARAnchorUpdatedEvent;
        public static event ARAnchorRemoved ARAnchorRemovedEvent;
        public static event ARAnchorAdded ARUserAnchorAddedEvent;
        public static event ARAnchorUpdated ARUserAnchorUpdatedEvent;
        public static event ARAnchorRemoved ARUserAnchorRemovedEvent;
		public static event ARSessionCallback ARSessionInterruptedEvent;
        public static event ARSessionCallback ARSessioninterruptionEndedEvent;
		public static event ARSessionTrackingChanged ARSessionTrackingChangedEvent;

```


"/Assets/Plugins/iOS/UnityARKit/NativeInterface/AR*.cs" - these are the scripting API equivalents of data structures exposed by ARKit

"/Assets/Plugins/iOS/UnityARKit/Utility/UnityARAnchorManager.cs" - this is a utility that keeps track of the anchor updates from ARKit and can create corresponding Unity gameobjects for them (see GeneratePlanes.cs component on how to use it)

"/Assets/Plugins/iOS/UnityARKit/Editor/UnityARBuildPostprocessor.cs" - this is an editor script that runs at build time on iOS 

## ARKit useful components: ##

"/Assets/Plugins/iOS/UnityARKit/UnityARCameraManager.cs" - this component should be placed on a gameobject in the scene that references the camera that you want to control via ARKit, and it will position and rotate the camera as well as provide the correct projection matrix to it based on updates from ARKit.  This component also has the code to initialize an ARKit session.

"/Assets/Plugins/iOS/UnityARKit/UnityARVideo.cs" - this component should be placed on the camera and grabs the textures needed for rendering the video, and sets it on the material needed for blitting to the backbuffer, and sets up the command buffer to do the actual blit

"/Assest/Plugins/iOS/UnityARKit/UnityARUserAnchorComponent.cs" - this component adds and removes Anchors from ARKit based on the lifecycle of the GameObject it's added to.

You should be able to build the UnityARKitScene.unity to iOS to get a taste of what ARKit is capable of.  It demostrates all the basic functionality of the ARKit in this scene.  

See TUTORIAL.txt in this project for more detailed steps on setting up a project step by step.

Please feel free to extend the plugin and send pull requests. You may also provide feedback if you would like improvements or want to suggest changes.  Happy coding and have fun!