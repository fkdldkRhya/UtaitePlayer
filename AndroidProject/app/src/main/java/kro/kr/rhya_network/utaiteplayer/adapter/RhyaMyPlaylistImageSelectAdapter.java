package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaImageData;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayListImageManager;

public class RhyaMyPlaylistImageSelectAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    public final ArrayList<RhyaImageData> listMain;
    private Context context;
    // Select pos
    public int selectedPos = -1;


    public RhyaMyPlaylistImageSelectAdapter() {
        RhyaPlayListImageManager rhyaPlayListImageManager = new RhyaPlayListImageManager();
        listMain = rhyaPlayListImageManager.getImageData();
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_playlist_image_select, parent, false);
        context = view.getContext();

        return new RhyaMyPlaylistImageSelectAdapter.ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        ((RhyaMyPlaylistImageSelectAdapter.ViewHolder) holder).onBind(listMain.get(position));
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList() {
        notifyDataSetChanged();
    }

    @Override
    public int getItemCount() {
        return listMain.size();
    }

    @Override
    public int getItemViewType(int position) {
        return position;
    }

    class ViewHolder extends RecyclerView.ViewHolder {
        private final ImageView imageView;
        private final ImageView imageViewOnlyBorder;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            imageView = itemView.findViewById(R.id.imageView);
            imageViewOnlyBorder = itemView.findViewById(R.id.imageViewOnlyBorder);

            itemView.setOnClickListener(view -> {
                int pos = getAbsoluteAdapterPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    RhyaImageData rhyaImageData = listMain.get(pos);

                    if (selectedPos == pos) {
                        selectedPos = -1;
                        rhyaImageData.setSelected(false);

                        notifyItemChanged(pos);

                        return;
                    }

                    if (selectedPos != -1) {
                        RhyaImageData rhyaImageDataSelected = listMain.get(selectedPos);
                        rhyaImageDataSelected.setSelected(false);

                        notifyItemChanged(selectedPos);
                    }

                    selectedPos = pos;

                    rhyaImageData.setSelected(!rhyaImageData.isSelected());

                    notifyItemChanged(pos);
                }
            });
        }

        public void onBind(RhyaImageData image) {
            Glide.with(context)
                    .load(image.getImage())
                    .placeholder(R.drawable.app_logo_for_black)
                    .error(R.drawable.app_logo_for_black)
                    .fallback(R.drawable.app_logo_for_black)
                    .into(imageView);

            if (image.isSelected()) {
                imageViewOnlyBorder.setVisibility(View.VISIBLE);
            }else {
                imageViewOnlyBorder.setVisibility(View.GONE);
            }
        }
    }
}
