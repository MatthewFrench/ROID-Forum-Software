library DrawController;
import 'package:three/three.dart';
import 'dart:html';
import 'BlockGameController.dart';

class DrawController {
  BlockGameController blockGame;
  PerspectiveCamera camera, minimapCamera;
  Scene scene;
  WebGLRenderer renderer;
  //PointLight light;
  ShaderMaterial blockMaterialShader;
  MeshBasicMaterial playerMaterial;
  CubeGeometry playerGeometry;
  Map<num, CubeGeometry> blockGeometries = new Map();
  Stopwatch stopwatch;//, minimapStopwatch;

  DrawController(BlockGameController bg) {
    blockGame = bg;
    scene = new Scene();

    camera = new PerspectiveCamera(70.0, window.innerWidth / window.innerHeight, 1.0, 1000.0);
    camera.position.z = 400.0;
    scene.add(camera);

    minimapCamera = new PerspectiveCamera(70.0, window.innerWidth / window.innerHeight, 1.0, 3000.0);
    minimapCamera.position.z = 2000.0;
    scene.add(minimapCamera);

    //light = new PointLight(0xFFFFFF, intensity: 3.0);
    //light.position.setValues(0.0, 0.0, 400.0);
    //light.castShadow = false;
    //light.receiveShadow = false;
    //scene.add(light);

    renderer = new WebGLRenderer();
    renderer.setSize(window.innerWidth, window.innerHeight);
    renderer.domElement.style.position = "absolute";
    bg.guiController.container.nodes.add(renderer.domElement);

    playerMaterial = new MeshBasicMaterial(color: 0xF6FF00);
    playerGeometry = new CubeGeometry(20.0, 40.0, 20.0);

//MeshLambertMaterial material = new MeshLambertMaterial(color: color);
    //Map<String, Uniform> uniforms = { "amplitude": new Uniform.float(0.0) };
    blockMaterialShader = new ShaderMaterial(//uniforms: uniforms,
    vertexShader: getVertexShader(), vertexColors: VertexColors, fragmentShader: getFragmentShader());

    stopwatch = new Stopwatch();
    stopwatch.start();
    // minimapStopwatch = new Stopwatch();
    // minimapStopwatch.start();
  }
  //void draw(_) {
  //  window.requestAnimationFrame(draw);
  //  renderScene();
  //}
  void renderScene() {
    stopwatch.reset();
    if (blockGame.entityController.mainPlayer != null) {
      //Set the light and camera to be on the main player
      double x = blockGame.entityController.mainPlayer.player3DMesh.position.x;
      double y = blockGame.entityController.mainPlayer.player3DMesh.position.y;
      camera.position.setValues(x, y, camera.position.z);

      int width = window.innerWidth;
      int height = window.innerHeight;
      renderer.setViewport(0, 0, width, height);
      renderer.setScissor(0, 0, width, height);
      renderer.enableScissorTest(true);
      renderer.render(scene, camera);

      //if (minimapStopwatch.elapsedMilliseconds >= 1000) {
      //minimapStopwatch.reset();
      width = 200;
      height = 200;
      renderer.setViewport(window.innerWidth - width, 0, width, height);
      renderer.setScissor(window.innerWidth - width, 0, width, height);
      renderer.enableScissorTest(true);

      minimapCamera.aspect = width / height;
      minimapCamera.updateProjectionMatrix();
      minimapCamera.position.setValues(x, y, minimapCamera.position.z);
      renderer.render(scene, minimapCamera);
      //}
    } else {
      //Follow player 0 or just stare at nothing
      double x = 0.0;
      double y = 0.0;
      if (blockGame.entityController.players.length > 0) {
         double x = blockGame.entityController.players.first.player3DMesh.position.x;
         double y = blockGame.entityController.players.first.player3DMesh.position.y;
      }
      
      camera.position.setValues(x, y, camera.position.z);

      int width = window.innerWidth;
      int height = window.innerHeight;
      renderer.setViewport(0, 0, width, height);
      renderer.setScissor(0, 0, width, height);
      renderer.enableScissorTest(true);
      renderer.render(scene, camera);
    }
  }
  void addMeshToScene(Mesh mesh) {
    scene.add(mesh);
  }
  void removeMeshFromScene(Mesh mesh) {
    scene.remove(mesh);
  }
  Mesh createPlayer3DMesh() {
    Mesh characterCube = new Mesh(playerGeometry, playerMaterial);
    characterCube.castShadow = false;
    characterCube.receiveShadow = false;
    return characterCube;
  }
  Mesh createBlock3DMesh(int color) {
    CubeGeometry blockGeometry = blockGeometries[color];
    if (blockGeometry == null) {
      blockGeometry = new CubeGeometry(20.0, 20.0, 20.0);
      for (int i = 0; i < blockGeometry.faces.length; i++) {
        Face f = blockGeometry.faces[i];
        f.color.setHex(darkenHex(color, 0.9 + (i * 0.2)));
        if (i == 4) {
          f.color.setHex(color);
        }
        if (i == 5) {
          f.color.setHex(darkenHex(color, 0.95));
        }
      }
      blockGeometries[color] = blockGeometry;
    }
    Mesh cube = new Mesh(blockGeometry, blockMaterialShader);//material);
    cube.castShadow = false;
    cube.receiveShadow = false;
    return cube;
  }
  Mesh createBlockSelector3DMesh(num color) {
    MeshBasicMaterial material = new MeshBasicMaterial(color: color, wireframe: true, wireframeLinewidth: 2.0);
    CubeGeometry blockGeometry = new CubeGeometry(20.0, 20.0, 20.0);
    Mesh cube = new Mesh(blockGeometry, material);
    cube.castShadow = false;
    cube.receiveShadow = false;
    return cube;
  }
  String getVertexShader() {
    return "varying vec3 vColor;" "void main(){" "vColor = color;" "gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);" "}";
  }
  String getFragmentShader() {
    return "varying vec3 vColor;" "void main(){" "gl_FragColor = vec4( vColor.rgb, 1.0 );" "}";
  }
  num darkenHex(int color, double percent) {
    Color c = new Color(color);
    c.setRGB(c.r * percent, c.g * percent, c.b * percent);
    return c.getHex();
  }
}