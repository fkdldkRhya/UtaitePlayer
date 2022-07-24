package kro.kr.rhya_network.utaiteplayer.utils;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.ItemTouchHelper;
import androidx.recyclerview.widget.RecyclerView;

import kro.kr.rhya_network.utaiteplayer.adapter.RhyaMyPlayListInfoAdapter;

public class RhyaItemTouchHelperCallBack extends ItemTouchHelper.Callback {
    @Override
    public int getMovementFlags(@NonNull RecyclerView recyclerView, @NonNull RecyclerView.ViewHolder viewHolder) {
        if (viewHolder.getAbsoluteAdapterPosition() == 0 || viewHolder.getAbsoluteAdapterPosition() == 1)
            return 0;

        int dragFlags = ItemTouchHelper.UP | ItemTouchHelper.DOWN;
        int swipeFlags = ItemTouchHelper.START | ItemTouchHelper.END;

        return makeMovementFlags(dragFlags, swipeFlags);
    }

    @Override
    public boolean onMove(@NonNull RecyclerView recyclerView, @NonNull RecyclerView.ViewHolder viewHolder, @NonNull RecyclerView.ViewHolder target) {
        if (viewHolder.getAbsoluteAdapterPosition() == 0 || viewHolder.getAbsoluteAdapterPosition() == 1 || target.getAbsoluteAdapterPosition() == 0 || target.getAbsoluteAdapterPosition() == 1)
            return false;

        onItemMoveListener.onItemMove(viewHolder.getAbsoluteAdapterPosition(), target.getAbsoluteAdapterPosition());

        return true;
    }

    @Override
    public boolean isItemViewSwipeEnabled() {
        return false;
    }

    @Override
    public void onSwiped(@NonNull RecyclerView.ViewHolder viewHolder, int direction) {

    }

    public interface OnItemMoveListener {
        void onItemMove(int fromPosition, int toPosition);
    }

    private final OnItemMoveListener onItemMoveListener;
    private final RhyaMyPlayListInfoAdapter rhyaMyPlayListInfoAdapter;
    public RhyaItemTouchHelperCallBack(OnItemMoveListener onItemMoveListener, RhyaMyPlayListInfoAdapter rhyaMyPlayListInfoAdapter) {
        this.onItemMoveListener = onItemMoveListener;
        this.rhyaMyPlayListInfoAdapter = rhyaMyPlayListInfoAdapter;
    }
}
