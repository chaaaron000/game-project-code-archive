using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UICamera : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        // Add WriteLogMessage as a delegate of the RenderPipelineManager.beginCameraRendering event
        Debug.Log("UICamera OnEnable");
        RenderPipelineManager.beginCameraRendering += BeginRender;
        RenderPipelineManager.endCameraRendering += EndRender;
    }

    // Unity calls this method automatically when it disables this component
    private void OnDisable()
    {
        // Remove WriteLogMessage as a delegate of the  RenderPipelineManager.beginCameraRendering event
        RenderPipelineManager.beginCameraRendering -= BeginRender;
        RenderPipelineManager.endCameraRendering -= EndRender;
    }

    // When this method is a delegate of RenderPipeline.beginCameraRendering event, Unity calls this method every time it raises the beginCameraRendering event
    void BeginRender(ScriptableRenderContext context, Camera camera)
    {
        // Write text to the console
        //Debug.Log($"Beginning rendering the camera: {camera.name}");
        if(camera.name == "UICamera")
        {
            //Debug.Log("Turn off fog");
            RenderSettings.fog = false;
        }
        
    }

    void EndRender(ScriptableRenderContext context, Camera camera)
    {
        //Debug.Log($"Ending rendering the camera: {camera.name}");
        if(camera.name == "UICamera")
        {
            //Debug.Log("Turn on fog");
            RenderSettings.fog = true;
        }
    }
}
