package kro.kr.rhya_network.utaiteplayer.utils;

import android.content.Context;
import android.graphics.Rect;
import android.util.AttributeSet;

public class RhyaMarqueeTextView extends androidx.appcompat.widget.AppCompatTextView {

    String TAG = "MarqueeTextView" ;

    public RhyaMarqueeTextView(Context context) {
        super(context);
    }

    public RhyaMarqueeTextView(Context context, AttributeSet attributeSet) {
        super(context, attributeSet);
    }

    @Override
    protected void onFocusChanged(boolean focused, int direction, Rect
            previouslyFocusedRect) {

        //Log.d(TAG, "getMarqueeRepeatLimit onFocusChanged(" + this.getMarqueeRepeatLimit() + ")") ;

        if(focused)
            super.onFocusChanged(true, direction, previouslyFocusedRect);

    }

    @Override
    public void onWindowFocusChanged(boolean focused) {

        //Log.d(TAG, "getMarqueeRepeatLimit onWindowFocusChanged (" + this.getMarqueeRepeatLimit() + ")") ;

        if(focused)
            super.onWindowFocusChanged(true);
    }

    @Override
    public boolean isFocused() {
        return true;
    }
}
