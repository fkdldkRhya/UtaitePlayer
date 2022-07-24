package kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.provider.interfaces;

import androidx.annotation.StyleableRes;

/**
 * Created by Amar Jain on 17/03/17.
 */

public interface IResourceProvider {
    int getResourceId(@StyleableRes final int styleResId);
    void recycle();
}
