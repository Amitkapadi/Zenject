using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif
using UnityEngine;
#endif

namespace Zenject.Internal
{
    public class ZenUtilInternal
    {
        // Due to the way that Unity overrides the Equals operator,
        // normal null checks such as (x == null) do not always work as
        // expected
        // In those cases you can use this function which will also
        // work with non-unity objects
        public static bool IsNull(System.Object obj)
        {
            return obj == null || obj.Equals(null);
        }

#if !ZEN_NOT_UNITY3D
        // NOTE: This method will not return components that are within a FacadeCompositionRoot
        public static IEnumerable<Component> GetInjectableComponentsBottomUp(
            GameObject gameObject, bool recursive, bool includeInactive)
        {
            if (!gameObject.activeInHierarchy && !includeInactive)
            {
                yield break;
            }

            var compRoot = gameObject.GetComponent<FacadeCompositionRoot>();

            if (compRoot != null)
            {
                foreach (var component in gameObject.GetComponents<Component>())
                {
                    if (compRoot.Facade == component)
                    {
                        // Allow an exception for the facade class to exist alongside the facade composition root
                        // on the same game object
                        // And inject all other components from the parent root
                        continue;
                    }

                    yield return component;
                }

                yield break;
            }

            if (recursive)
            {
                foreach (Transform child in gameObject.transform)
                {
                    foreach (var component in GetInjectableComponentsBottomUp(child.gameObject, recursive, includeInactive))
                    {
                        yield return component;
                    }
                }
            }

            foreach (var component in gameObject.GetComponents<Component>())
            {
                yield return component;
            }
        }
#endif
    }
}
