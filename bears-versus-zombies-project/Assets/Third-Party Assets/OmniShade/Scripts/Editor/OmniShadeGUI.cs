//------------------------------------
//             OmniShade
//     Copyright© 2025 OmniShade     
//------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

/**
 * This class manages the GUI for the shader, automatically enabling/disabling keywords when values change.
 **/
public class OmniShadeGUI : ShaderGUI {	
	// Shader keywords which are automatically enabled/disabled based on usage
	readonly List<(string keyword, string name, PropertyType type, Vector4 defaultValue)> props = new List<(string keyword, string name, PropertyType type, Vector4 defaultValue)>() {
		("TOP_TEX", "_TopTex", PropertyType.Texture, Vector4.one),							// Triplanar
		("TRIPLANAR_SHARPNESS", "_TriplanarSharpness", PropertyType.Float, Vector4.one),	// Triplanar
		("BASE_CONTRAST", "_Contrast", PropertyType.Float, Vector4.one),
		("BASE_SATURATION", "_Saturation", PropertyType.Float, Vector4.one), 
		("SPECULAR_MAP", "_SpecularTex", PropertyType.Texture, Vector4.one),
		("RIM_DIRECTION", "_RimDirection", PropertyType.Vector, Vector4.zero),
		("REFLECTION_TEX", "_ReflectionTex", PropertyType.Texture, Vector4.one),
		("NORMAL_MAP", "_NormalTex", PropertyType.Texture, Vector4.one),
		("NORMAL_MAP2", "_NormalTex2", PropertyType.Texture, Vector4.one),
		("NORMAL_MAP_TOP", "_NormalTopTex", PropertyType.Texture, Vector4.one),				// Triplanar
		("LIGHT_MAP", "_LightmapTex", PropertyType.Texture, Vector4.one),
		("EMISSIVE_MAP", "_EmissiveTex", PropertyType.Texture, Vector4.one),
		("MATCAP", "_MatCapTex", PropertyType.Texture, Vector4.one),	
		("MATCAP_CONTRAST", "_MatCapContrast", PropertyType.Float, Vector4.one),
		("VERTEX_COLORS_CONTRAST", "_VertexColorsContrast", PropertyType.Float, Vector4.one),
		("DETAIL", "_DetailTex", PropertyType.Texture, Vector4.one),
		("DETAIL_CONTRAST", "_DetailContrast", PropertyType.Float, Vector4.one),
		("LAYER1", "_Layer1Tex", PropertyType.Texture, Vector4.one),
		("LAYER2", "_Layer2Tex", PropertyType.Texture, Vector4.one),
		("LAYER3", "_Layer3Tex", PropertyType.Texture, Vector4.one),
		("TRANSPARENCY_MASK", "_TransparencyMaskTex", PropertyType.Texture, Vector4.one),
		("TRANSPARENCY_MASK_CONTRAST", "_TransparencyMaskContrast", PropertyType.Float, Vector4.one),
		("HEIGHT_COLORS_TEX", "_HeightColorsTex", PropertyType.Texture, Vector4.one),
        ("AMBIENT", "_AmbientBrightness", PropertyType.Float, Vector4.zero),
        ("SHADOW_OVERLAY", "_ShadowOverlayTex", PropertyType.Texture, Vector4.one),
		("ANIME_SOFT", "_AnimeSoftness", PropertyType.Float, Vector4.zero),
		("ZOFFSET", "_ZOffset", PropertyType.Float, Vector4.zero),
	};

	// Parameters that are ON by default
	readonly List<(string keyword, string name)> defaultOnParams = new List<(string keyword, string name)>() {
		("DIFFUSE", "_Diffuse" ),
		("MATCAP_PERSPECTIVE", "_MatCapPerspective" ),
		("AMBIENT", "_Ambient" ),
		("FOG", "_Fog" ),
		("SHADOWS_ENABLED", "_ShadowsEnabled" ),
	};

	enum PropertyType {
		Float, Vector, Texture
	};

	struct PropertyHeader {
		public string headerName;
		public bool isOpen;
		public PropertyHeader(string _header, bool _isOpen) {
			this.headerName = _header;
			this.isOpen = _isOpen;
		}
	}; 

	const string HEADER_GROUP = "HeaderGroup";

	enum ExpandType {
		All,
		Active,
		Collapse,
		Keep,
	};

	ExpandType forceExpand = ExpandType.Active;
	int prevPreset = -1;
	List<Material> prevSelectedMats = new List<Material>();
	readonly Dictionary<string, PropertyHeader> propertyHeaders = new Dictionary<string, PropertyHeader>();

	static readonly Dictionary<string, GUIContent> toolTipsCache = new Dictionary<string, GUIContent>();

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
		this.RenderGUI(materialEditor, properties);

		// Loop selected materials
		var mats = this.GetSelectedMaterials(materialEditor);
		foreach (var material in mats) {
			this.AutoEnableShaderKeywords(material);
			this.UpdatePresetValues(material);

			// Reset previous preset if multi-selection
			if (mats.Count > 1)
				this.prevPreset = -1;
		}

		// Reset preset if selection changed
		if (mats.Count == 1 && this.prevSelectedMats.Count == 1 &&
			mats[0] != null && this.prevSelectedMats[0] != null && mats[0].name != this.prevSelectedMats[0].name)
			this.prevPreset = -1;
		this.prevSelectedMats = mats;
	}

	List<Material> GetSelectedMaterials(MaterialEditor materialEditor) {
		var mat = materialEditor.target as Material;
		var mats = new List<Material>();
		if (mat != null)
			mats.Add(mat);
		foreach (var selected in Selection.objects) {
			if (selected.GetType() == typeof(Material)) {
				var selectedMat = selected as Material;
				if (selectedMat != mat && selectedMat != null &&
					selectedMat.shader.name.Contains(OmniShade.NAME))
					mats.Add(selectedMat);
			}
		}
		return mats;
	}

	void AutoEnableShaderKeywords(Material mat) {
		foreach (var prop in this.props) {
			if (!mat.HasProperty(prop.name))
				continue;

			// Check if property value is being used (not set to default)
			bool isInUse = false;
			switch (prop.type) {
				case PropertyType.Float:
					isInUse = mat.GetFloat(prop.name) != prop.defaultValue.x;
					break;
				case PropertyType.Vector:
					isInUse = mat.GetVector(prop.name) != prop.defaultValue;
					break;
				case PropertyType.Texture:
					isInUse = mat.GetTexture(prop.name) != null;
					break;
				default:
					break;
			}

			// Enable or disable shader keyword
			if (isInUse) {
				if (!mat.IsKeywordEnabled(prop.keyword))
					mat.EnableKeyword(prop.keyword);
			} 
			else if (mat.IsKeywordEnabled(prop.keyword))
				mat.DisableKeyword(prop.keyword);
		}

		// Set keywords for parameters that are ON by default
		foreach (var defaultOnParam in this.defaultOnParams) {
			if (mat.HasProperty(defaultOnParam.name) && mat.GetFloat(defaultOnParam.name) == 1)
				mat.EnableKeyword(defaultOnParam.keyword);
		}

		this.AutoConfigureProperties(mat);
	}

	void AutoConfigureProperties(Material mat) {
		// MatCap Static Rotation default angle points to camera
		if (mat.IsKeywordEnabled("MATCAP_STATIC") && mat.HasProperty("_MatCapRot") && 
			mat.GetVector("_MatCapRot") == Vector4.zero) {
			var cam = GameObject.FindFirstObjectByType<Camera>();
			if (cam != null) {
				var matCapRot = -cam.transform.rotation.eulerAngles * Mathf.PI / 180;
				mat.SetVector("_MatCapRot", matCapRot);
			}
		}

		// Camera fade
		float fadeStart = mat.HasProperty("_CameraFadeStart") ? mat.GetFloat("_CameraFadeStart") : 0;
		float fadeEnd = mat.HasProperty("_CameraFadeEnd") ? mat.GetFloat("_CameraFadeEnd") : 0;
		bool cameraFadeEnabled = fadeStart < fadeEnd;
		EnableDisableKeyword(mat, cameraFadeEnabled, "CAMERA_FADE");

		// Enable/disable outline pass
		bool outlineEnabled = mat.GetFloat("_Outline") == 1;
		string outlinePassName = mat.shader.name.Contains("URP") ? "SRPDefaultUnlit" : "Always";
		if (outlineEnabled != mat.GetShaderPassEnabled(outlinePassName))
			mat.SetShaderPassEnabled(outlinePassName, outlineEnabled);
		EnableDisableKeyword(mat, !outlineEnabled, "OUTLINE_PASS_DISABLED");
		if (mat.GetInt("_OutlineComp") == 6) {	// Interior outline: Show
			// Auto-set OutlineGroup to non-zero value if hiding interior outlines
			if (mat.GetInt("_OutlineGroup") == 0)
				mat.SetInt("_OutlineGroup", 1);
			mat.SetInt("_OutlinePass", 2);  	// Stencil pass: Replace
		}
		else
			mat.SetInt("_OutlinePass", 0);  	// Stencil pass: Keep

		// Enable GPU instancing automatically for certain keywords
		var instancingParams = new List<string>() { 
			"_Plant",
		};
		if (!mat.enableInstancing) {
			foreach (var instancingParma in instancingParams) {
				if ((mat.HasProperty(instancingParma) && mat.GetFloat(instancingParma) == 1))
					mat.enableInstancing = true;
			}
		}

		// Global illumination floag
		if (mat.GetVector("_Emissive") != Vector4.zero) {
			if (mat.globalIlluminationFlags != MaterialGlobalIlluminationFlags.BakedEmissive)
				mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
		}
		else {
			if (mat.globalIlluminationFlags != MaterialGlobalIlluminationFlags.EmissiveIsBlack)
				mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
		}
	}

	void EnableDisableKeyword(Material mat, bool value, string keyword) {
		if (value) {
			if (!mat.IsKeywordEnabled(keyword))
				mat.EnableKeyword(keyword);
		}
		else {
			if (mat.IsKeywordEnabled(keyword))
				mat.DisableKeyword(keyword);
		}
	}
	
	void UpdatePresetValues(Material mat) {
		int preset = (int)mat.GetFloat("_Preset");

		// Do nothing if preset was not changed
		if (this.prevPreset == -1 || this.prevPreset == preset) {
			this.prevPreset = preset;
			return;
		}

		mat.SetFloat("_Cull", 2);                   // Back
		mat.SetFloat("_ZTest", 4);                  // LessEqual
		mat.SetFloat("_BlendOp", 0);                // Add
		switch (preset) {
			case 0:                                 // OPAQUE
				mat.SetFloat("_ZWrite", 1);
				mat.SetFloat("_SourceBlend", 1);    // One
				mat.SetFloat("_DestBlend", 0);      // Zero
				if (mat.renderQueue >= 2450)
					mat.renderQueue = 2000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 1:                                 // TRANSPARENT
				mat.SetFloat("_ZWrite", 0);
				mat.SetFloat("_SourceBlend", 5);    // SrcAlpha
				mat.SetFloat("_DestBlend", 10);     // OneMinusSrcAlpha
				if (mat.renderQueue < 3000)
					mat.renderQueue = 3000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 2:                                 // TRANSPARENT ADDITIVE
				mat.SetFloat("_ZWrite", 0);
				mat.SetFloat("_SourceBlend", 1);    // One
				mat.SetFloat("_DestBlend", 1);      // One
				if (mat.renderQueue < 3000)
					mat.renderQueue = 3000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 3:                                 // TRANSPARENT ADDITIVE ALPHA
				mat.SetFloat("_ZWrite", 0);
				mat.SetFloat("_SourceBlend", 5);    // SrcAlpha
				mat.SetFloat("_DestBlend", 1);      // One
				if (mat.renderQueue < 3000)
					mat.renderQueue = 3000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 4:                                 // OPAQUE CUTOUT
				mat.SetFloat("_Cull", 0);           // Disabled
				mat.SetFloat("_ZWrite", 1);
				mat.SetFloat("_SourceBlend", 1);    // One
				mat.SetFloat("_DestBlend", 0);      // Zero
				mat.SetFloat("_Cutout", 1);         // Cutout
				mat.EnableKeyword("CUTOUT");
				if (mat.renderQueue < 2450 || mat.renderQueue >= 3000)
					mat.renderQueue = 2450;
				break;

			default:
				Debug.LogError(OmniShade.NAME + ": Unrecognized Preset (" + preset + ")");
				break;
		}

		this.prevPreset = preset;
	}
	
	void RenderGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
		materialEditor.SetDefaultGUIWidths();

		// Documentation button
		var content = new GUIContent(EditorGUIUtility.IconContent("_Help")) {
			text = OmniShade.NAME + " Docs",
			tooltip = OmniShade.DOCS_URL
		};
      	if (GUILayout.Button(content))
		  Help.BrowseURL(OmniShade.DOCS_URL);
		
		// Expand/Close all buttons
		GUILayout.BeginHorizontal();
		var expandAll = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus")) { text = "Expand All" };
		if (GUILayout.Button(expandAll))
			this.forceExpand = ExpandType.All;
		if (GUILayout.Button("Expand Active"))
			this.forceExpand = ExpandType.Active;
		var collapse = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus")) { text = "Collapse" };
		if (GUILayout.Button(collapse))
			this.forceExpand = ExpandType.Collapse;
		GUILayout.EndHorizontal();

		string currentHeaderName = this.RenderShaderProperties(materialEditor, properties);

		// Append Unity rendering options to Rendering group
		if (currentHeaderName == "Rendering") {
			materialEditor.EnableInstancingField();
			materialEditor.DoubleSidedGIField();
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		// Upgrade button
		#if LITE
			GUILayout.Space(10);
			if (GUILayout.Button("Upgrade to " + OmniShade.NAME + " Pro"))
				Help.BrowseURL(OmniShade.PRO_URL);
		#endif
	}

	string RenderShaderProperties(MaterialEditor materialEditor, MaterialProperty[] properties) {
		this.RefreshPropertyHeaders(materialEditor);

		bool isFoldoutOpen = true;
		string currentHeaderName = string.Empty;
		int uvTileCount = 0;
		foreach (var prop in properties) {
			if (((uint)prop.flags & (uint)MaterialProperty.PropFlags.HideInInspector) == 1)
				continue;

			// If start of header, begin a new foldout group
			if (this.propertyHeaders.ContainsKey(prop.name)) {
				// Close previous foldout group
				if (!string.IsNullOrEmpty(currentHeaderName)) {
					// Append Unity rendering options to end of groups
					if (isFoldoutOpen) {
						if (currentHeaderName == "Culling And Blending")
							materialEditor.RenderQueueField();
						else if (currentHeaderName == "Rendering") {
							materialEditor.EnableInstancingField();
							materialEditor.DoubleSidedGIField();
						}
					}
					EditorGUILayout.EndFoldoutHeaderGroup();
				}

				// Begin foldout header
				var header = this.propertyHeaders[prop.name];
				currentHeaderName = header.headerName;
				isFoldoutOpen = header.isOpen = this.BeginFoldoutHeader(header.isOpen, header.headerName);
				this.propertyHeaders[prop.name] = header;
			}

			// Render shader property
			if (isFoldoutOpen) {
				if (prop.name.StartsWith("_UVTileV")) {
					this.RenderUVTileDiscardProperty(materialEditor, prop, uvTileCount);
					uvTileCount++;
				}
				else
					this.RenderShaderProperty(materialEditor, prop);		
			}
		}

		return currentHeaderName;
	}

	void RenderUVTileDiscardProperty(MaterialEditor materialEditor, MaterialProperty prop, int uvTileCount) {
		if (uvTileCount % 4 == 0) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(prop.displayName);
		}

		float val = GUILayout.Toggle(prop.floatValue > 0, string.Empty) ? 1.0f : 0.0f;
		if (val != prop.floatValue) {
			var mats = this.GetSelectedMaterials(materialEditor);
			foreach (var mat in mats)
				mat.SetFloat(prop.name, val);
		}

		if ((uvTileCount + 1) % 4 == 0)
			EditorGUILayout.EndHorizontal();
	}

	void RenderShaderProperty(MaterialEditor materialEditor, MaterialProperty prop) {
		string label = prop.displayName;
		var content = this.GetTooltip(label);
		materialEditor.ShaderProperty(prop, content);
	}
	
	void RefreshPropertyHeaders(MaterialEditor materialEditor) {
		var mat = materialEditor.target as Material;
		var shader = mat.shader;
		int numProps = shader.GetPropertyCount();

		var headerActiveDic = GetActivePropertyHeaders(mat);

		// Update property headers and check for new headers
		var newProps = new List<string>();
		for (int i = 0; i < numProps; i++) {
			var propAttrs = shader.GetPropertyAttributes(i);
			for (int j = 0; j < propAttrs.Length; j++) {
				// Skip if not a header attribute
				string propAttr = propAttrs[j];
				if (!this.IsHeaderGroup(propAttr))
					continue;

				string propName = shader.GetPropertyName(i);
				string headerName = this.GetHeaderGroupName(propAttr);
				newProps.Add(propName);

				// Update cache if something changed
				if (!this.propertyHeaders.ContainsKey(propName) ||
					this.propertyHeaders[propName].headerName != headerName || this.forceExpand != ExpandType.Keep) {
					bool isOpen = this.forceExpand == ExpandType.All;
					if (!this.propertyHeaders.ContainsKey(propName)) {          // New entry
						if (this.forceExpand == ExpandType.Keep || this.forceExpand == ExpandType.Active)
							isOpen = !headerActiveDic.ContainsKey(headerName) || headerActiveDic[headerName];
						var header = new PropertyHeader(headerName, isOpen);
						this.propertyHeaders.Add(propName, header);
					}
					else {                                                      // Update existing entry
						if (this.forceExpand == ExpandType.Keep)
							isOpen = this.propertyHeaders[propName].isOpen;
						else if (this.forceExpand == ExpandType.Active)
							isOpen = !headerActiveDic.ContainsKey(headerName) || headerActiveDic[headerName];
						var header = new PropertyHeader(headerName, isOpen);
						this.propertyHeaders[propName] = header;
					}
				}
			}
		}
		this.forceExpand = ExpandType.Keep;

		// Remove any headers that were deleted
		var propNames = new List<string>();
		foreach (var propName in this.propertyHeaders.Keys)
			propNames.Add(propName);
		var deletedProps = propNames.Except(newProps);
		foreach (var deletedProp in deletedProps)
			this.propertyHeaders.Remove(deletedProp);
	}

	Dictionary<string, bool> GetActivePropertyHeaders(Material mat) {
		var defaultOnHeaders = new string[] {
			"Culling And Blending"
		};

		var shader = mat.shader;
		int numProps = shader.GetPropertyCount();
		var headerActiveDic = new Dictionary<string, bool>();
		if (this.forceExpand == ExpandType.Active) {
			string currentHeaderName = string.Empty;
			for (int i = 0; i < numProps; i++) {
				// Check if header group
				var propAttrs = shader.GetPropertyAttributes(i);
				string headerName = this.GetHeaderGroupName(propAttrs);
				if (!string.IsNullOrEmpty(headerName))
					currentHeaderName = headerName;

				// Skip if no headers found yet
				if (string.IsNullOrEmpty(currentHeaderName))
					continue;

				// Check active headers
				bool isInUse = defaultOnHeaders.Contains(currentHeaderName) || this.IsPropertyActive(mat, i);
				if (headerActiveDic.ContainsKey(currentHeaderName))
					headerActiveDic[currentHeaderName] |= isInUse;
				else
					headerActiveDic.Add(currentHeaderName, isInUse);
			}
		}

		return headerActiveDic;
	}

	bool IsPropertyActive(Material mat, int propertyIndex) {
		var shader = mat.shader;
		string propName = shader.GetPropertyName(propertyIndex);
		string propDesc = shader.GetPropertyDescription(propertyIndex);
		var propType = shader.GetPropertyType(propertyIndex);

		if (!mat.HasProperty(propName))
			return false;
		if ((propType == ShaderPropertyType.Float && propDesc.StartsWith("Enable") && mat.GetFloat(propName) == 1) ||
			(propType == ShaderPropertyType.Texture && mat.GetTexture(propName) != null) ||
			(propName == "_Emissive" && (Vector3)mat.GetVector("_Emissive") != Vector3.zero) ||
			(propName == "_AmbientBrightness" && mat.GetFloat("_AmbientBrightness") != 0) ||
			(propName.StartsWith("_Opt") && mat.GetFloat(propName) != 0) ||
			(propName == "_CameraFadeStart" && mat.IsKeywordEnabled("CAMERA_FADE")))
			return true;
		return false;
	}

	bool IsHeaderGroup(string propAttr) {
		return propAttr.StartsWith(HEADER_GROUP);
	}

	string GetHeaderGroupName(string[] propAttrs) {
		for (int j = 0; j < propAttrs.Length; j++) {
			string propAttr = propAttrs[j];
			if (this.IsHeaderGroup(propAttr)) {
				return GetHeaderGroupName(propAttr);
			}
		}
		return null;
	}

	string GetHeaderGroupName(string header) {
		int headerGroupLen = HEADER_GROUP.Length + 1;
		return header.Substring(headerGroupLen, header.LastIndexOf(")") - headerGroupLen);
	}

	bool BeginFoldoutHeader(bool isOpen, string label) {
		var content = this.GetTooltip(label);
		var defaultColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(1.35f, 1.35f, 1.35f);
		isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, content);
		GUI.backgroundColor = defaultColor;
		return isOpen;
	}

	GUIContent GetTooltip(string label) {
		// Check cache first
		if (OmniShadeGUI.toolTipsCache.ContainsKey(label))
			return OmniShadeGUI.toolTipsCache[label];

		string tooltip;
		switch (label) {
			case "Ignore Main Texture Alpha": tooltip = "Ignore the alpha channel on the texture, forcing it to be opaque."; break;
			case "Use UV For Sides": tooltip = "Use UV coordinates instead of triplanar for the side texturing."; break;
			case "Triplanar Blend Sharpness": tooltip = "The blend sharpness between the sides and top texture."; break;
			case "Diffuse Softness": tooltip = "Wraps the diffuse lighting around to make it softer."; break;
			case "Per-Pixel Point Lights": tooltip = "Compute point lights in fragment shader instead of vertex shader for higher quality. Slower, but useful on low-poly objects."; break;
			case "Enable Baked and Dynamic Lights": tooltip = "Toggle when using both a baked realtime light sources."; break;
			case "Specular Hair": tooltip = "Computes specular along tangents for hair or brushed metal highlights. In this mode, the Specular Map is used as a roughness map for the highlights as well."; break;
			case "Rim Direction": tooltip = "Rim light direction in world space."; break;
			case "Reflection": tooltip = "Uses Environment Skybox Material from Lighting Settings unless Reflection Cubemap specified."; break;
			case "Enable Reflection": tooltip = "Uses Environment Skybox Material from Lighting Settings unless Reflection Cubemap specified."; break;
			case "Mask With Rim": tooltip = "Mask the reflection with the rim's fresnel effect to simulate reflections only at glancing angles."; break;
			case "Perspective Correction": tooltip = "Improves accuracy for dynamic cameras. For stationary cameras, disable to improve performance."; break;
			case "Use Static Rotation": tooltip = "Lock the MatCap rotation to a static angle."; break;
			case "Apply To Lighting": tooltip = "Apply the Detail Map as a mask to the lighting instead, useful for effects like glitter."; break;
			case "Mask With Vertex Color (A)": tooltip = "Mask with the vertex color's alpha channel."; break;
			case "Mask With Vertex Color (R)": tooltip = "Mask with the vertex color's red channel."; break;
			case "Mask With Vertex Color (G)": tooltip = "Mask with the vertex color's green channel."; break;
			case "Mask With Vertex Color (B)": tooltip = "Mask with the vertex color's blue channel."; break;
			case "Transparency Mask": tooltip = "Use a texture (either red or alpha channel) to control the transparency of the object."; break;
			case "Height Based Colors": tooltip = "Apply a color to the object based on its local or world-space height."; break;
			case "Enable Height Based Colors": tooltip = "Apply a color to the object based on its local or world-space height."; break;
			case "Coordinate Space": tooltip = "World space is the final position after transforms, Local space is as it was modeled."; break;
			case "Shadow Overlay": tooltip = "Overlay a texture using world space coordinates, useful for simulating clouds over multiple ojects."; break;
			case "Animation Type": tooltip = "Scroll to continuously slide the texture, Sway to ping-pong."; break;
			case "Plant Sway": tooltip = "Animates the object gently back and forth to simulate wind movement."; break;
			case "Enable Plant Sway": tooltip = "Animates the object gently back and forth to simulate wind movement."; break;
			case "Phase Variation": tooltip = "A greater value means less synchronicity in the animation of objects."; break;
			case "Base Height": tooltip = "The height in local space from which the sway occurs when Plant Type set to Plant."; break;
			case "Plant Type": tooltip = "Plant will sway as if the object were anchored at the base height. Leaf has no anchor. Vertex Color Alpha sways more with higher alpha values."; break;
			case "Outline": tooltip = "Adds an outline silhouette around the object."; break;
			case "Enable Outline": tooltip = "Adds an outline silhouette around the object."; break;
			case "Outline Width": tooltip = "Width of the outline. Recommend not setting this too large."; break;
			case "Outline Width Camera-Independent": tooltip = "Width does not vary based on camera depth."; break;
			case "Interior Outlines": tooltip = "Show or hide interior outlines."; break;
			case "Outline Group": tooltip = "Group object outlines, used with Interior Outlines."; break;
			case "Anime": tooltip = "Anime-style ramp-lighting."; break;
			case "Enable Anime": tooltip = "Anime-style ramp-lighting."; break;
			case "Color 1": tooltip = "First ramp (shadow) color."; break;
			case "Luminance Threshold 1": tooltip = "Luminance threshold between first and second ramp."; break;
			case "Color 2": tooltip = "Second ramp (midtone) color."; break;
			case "Luminance Threshold 2": tooltip = "Luminance threshold between second and third ramp."; break;
			case "Color 3": tooltip = "Third ramp (highlights) color."; break;
			case "Softness": tooltip = "Softness between color transitions."; break;
			case "Ambient Brightness": tooltip = "Intensity of the Environment Lighting from the Lighting Settings."; break;
			case "Culling And Blend Preset": tooltip = "Set cull and blend settings from presets."; break;
			case "Culling": tooltip = "Side of the geometry that is rendered."; break;
			case "Z Write": tooltip = "If enabled, this object occludes those behind it."; break;
			case "Z Test": tooltip = "Set to Always if this object should always render, even if behind others."; break;
			case "Depth Offset": tooltip = "Adjust the distance from the camera to tune visibility."; break;
			case "Cutout Transparency": tooltip = "Discards pixels with alpha less than 0.5. Performance may be slow on mobile."; break;
			case "Enable Flat Shading": tooltip = "Use a flat-shading style for a blocky low-poly look."; break;
			case "Enable UV Tile Discard": tooltip = "Discard vertices based on UV values, used for toggling portions of a model on/off."; break;
			default: tooltip = ""; break;
		}

		// Create tool tip and cache
		var content = new GUIContent() {
			text = label,
			tooltip = tooltip
		};
		OmniShadeGUI.toolTipsCache.Add(label, content);
		return content;
	}

	 public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader) {
		 // Convert texture mapping
		var textureMapping = new Dictionary<string, string>() {
			{ "_BaseMap", "_MainTex" },
			{ "_MainTex", "_MainTex" },
			{ "_MetallicGlossMap", "_SpecularTex" },
			{ "_BumpMap", "_NormalTex" },
			{ "_OcclusionMap", "_LightmapTex" },
			{ "_DetailAlbedoMap", "_DetailTex" },
			{ "_EmissionMap", "_EmissiveTex" },
		};
		var tilingOffsetMapping = new Dictionary<string, Vector4>();

		// Fetch textures from mapping
		var texToReplace = new Dictionary<string, Texture>();
		foreach (var texMap in textureMapping) {
			if (mat.HasProperty(texMap.Key) && mat.GetTexture(texMap.Key) != null) {
				if (!texToReplace.ContainsKey(texMap.Value)) {
					texToReplace.Add(texMap.Value, mat.GetTexture(texMap.Key));

					// Store tiling offset
					Vector4 tilingOffset;
					Vector2 tiling, offset;
					tiling = mat.GetTextureScale(texMap.Key);
					offset = mat.GetTextureOffset(texMap.Key);
					tilingOffset.x = tiling.x;
					tilingOffset.y = tiling.y;
					tilingOffset.z = offset.x;
					tilingOffset.w = offset.y;
					tilingOffsetMapping.Add(texMap.Value, tilingOffset);
				}
				mat.SetTexture(texMap.Key, null);
			}
		}

		// Get base color
		Vector4 baseColor = Vector4.one;
		if (mat.HasProperty("_BaseColor"))
			baseColor = mat.GetVector("_BaseColor");

		// Get emission color
		Vector4 emissive = Vector4.zero;
		if (mat.HasProperty("_Emissive"))
			emissive = mat.GetVector("_Emissive");
		if (mat.HasProperty("_EmissionColor") && mat.globalIlluminationFlags != MaterialGlobalIlluminationFlags.EmissiveIsBlack)
			emissive = mat.GetVector("_EmissionColor");

		// Replace shader
		base.AssignNewShaderToMaterial(mat, oldShader, newShader);

		// Re-enable outline pass
		string outlinePassName = mat.shader.name.Contains("URP") ? "SRPDefaultUnlit" : "Always";
		mat.SetShaderPassEnabled(outlinePassName, true);

		// Replace textures
		Vector2 baseTiling = Vector2.one, baseOffset = Vector2.zero;
		foreach (var texToRep in texToReplace) {
			var texName = texToRep.Key;
			if (mat.HasProperty(texName)) {
				mat.SetTexture(texName, texToRep.Value);
				if (tilingOffsetMapping.ContainsKey(texName)) {
					// Restore tiling offset
					Vector4 tilingOffset = tilingOffsetMapping[texName];
					Vector2 tiling = (Vector2)tilingOffset;
					Vector2 offset;
					offset.x = tilingOffset.z;
					offset.y = tilingOffset.w;
					mat.SetTextureScale(texName, tiling);
					mat.SetTextureOffset(texName, offset);

					// Store base tiling offset
					if (texName == "_MainTex") {
						baseTiling = tiling;
						baseOffset = offset;
					}
				}
			}
		}

		// If prev shader is Lit, apply base tiling offset to all tex
		if (oldShader.name.Contains("Lit") || oldShader.name == "Standard") {
			foreach (var texMap in textureMapping) {
				var texName = texMap.Value;
				if (mat.HasProperty(texName)) {
					mat.SetTextureScale(texName, baseTiling);
					mat.SetTextureOffset(texName, baseOffset);
				}
			}
		}

		// Replace Base Color
		if (oldShader.name.Contains("Lit"))
			mat.SetColor("_Color", baseColor);

		// Replace emission color
		if (mat.HasProperty("_Emissive"))
			mat.SetVector("_Emissive", emissive);
		
		this.forceExpand = ExpandType.Active;
	}
}
