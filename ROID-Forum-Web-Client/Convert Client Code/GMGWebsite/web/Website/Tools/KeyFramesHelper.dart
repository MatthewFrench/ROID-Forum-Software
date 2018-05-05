library KeyFrameHelper;
import 'dart:html';

class KeyFramesHelper {

// search the CSSOM for a specific -webkit-keyframe rule
  static CssKeyframesRule findKeyframesRule(String rule) {
    // gather all stylesheets into an array
    List<StyleSheet> styleSheets = document.styleSheets;

    // loop through the stylesheets
    for (int i = 0; i < styleSheets.length; ++i) {
      // loop through all the rules
      if (styleSheets[i] is CssStyleSheet) {
        CssStyleSheet cssSheet = styleSheets[i];
        for (int j = 0; j < cssSheet.cssRules.length; ++j) {
          if (cssSheet.cssRules[j].type == CssRule.KEYFRAMES_RULE) {
            CssKeyframesRule keyframesRule = cssSheet.cssRules[j];
            // find the -webkit-keyframe rule whose name matches our passed over parameter and return that rule
            if (keyframesRule.name == rule) return keyframesRule;
          }
        }
      }
    }

    // rule not found so make it
    return null;
  }

// remove old keyframes and add new ones
  static void changeRotation(String ruleName, int oldRot, int newRot) {
    // find our -webkit-keyframe rule
    CssKeyframesRule keyframes = findKeyframesRule(ruleName);

    // remove the existing 0% and 100% rules
    keyframes.deleteRule("0%");
    keyframes.deleteRule("100%");

    // create new 0% and 100% rules with random numbers
    keyframes.appendRule("0% { -webkit-transform: rotate(${oldRot}deg); }");
    keyframes.appendRule("100% { -webkit-transform: rotate(${newRot}deg); }");

    // assign the animation to our element (which will cause the animation to run)
    //document.getElementById('box').style.webkitAnimationName = anim;
  }
}
