package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVOv2;

public class RhyaMyPlayListAddSongAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    public ArrayList<RhyaMusicInfoVOv2> listMain;
    private final TextView songCount;
    private final int maxCount;
    private Context context;

    public RhyaMyPlayListAddSongAdapter(TextView songCount, int maxCount) {
        this.songCount = songCount;
        this.maxCount = 100 - (maxCount -2 );
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_my_playlist_add_song, parent, false);
        context = view.getContext();

        return new RhyaMyPlayListAddSongAdapter.ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        ((RhyaMyPlayListAddSongAdapter.ViewHolder) holder).onBind(listMain.get(position));
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaMusicInfoVOv2> listMain) {
        this.listMain = listMain;

        notifyDataSetChanged();
    }

    @Override
    public int getItemCount() {
        return listMain == null ? 0 : listMain.size();
    }

    @Override
    public int getItemViewType(int position) {
        return position;
    }

    class ViewHolder extends RecyclerView.ViewHolder {
        private final ImageView imageView;
        private final TextView name;
        private final TextView singer;
        private final View view;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            imageView = itemView.findViewById(R.id.imageView);
            name = itemView.findViewById(R.id.name);
            singer = itemView.findViewById(R.id.singer);

            view = itemView;

            itemView.setOnClickListener(view -> {
                int pos = getAbsoluteAdapterPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    RhyaMusicInfoVOv2 rhyaMusicInfoVOv2 = listMain.get(pos);
                    try {
                        if (rhyaMusicInfoVOv2.getIsPlay()) {
                            rhyaMusicInfoVOv2.setIsPlay(false);
                            notifyItemChanged(pos);

                            int count = Integer.parseInt(songCount.getText().toString().replace("개 선택됨", ""));

                            StringBuilder stringBuilder;
                            stringBuilder = new StringBuilder();

                            stringBuilder.append(count - 1);
                            stringBuilder.append("개 선택됨");

                            songCount.setText(stringBuilder.toString());
                        }else {
                            StringBuilder stringBuilder;
                            stringBuilder = new StringBuilder();

                            rhyaMusicInfoVOv2.setIsPlay(true);
                            notifyItemChanged(pos);

                            int count = Integer.parseInt(songCount.getText().toString().replace("개 선택됨", ""));

                            if (count >= maxCount) {
                                stringBuilder.append("최대 '");
                                stringBuilder.append(maxCount);
                                stringBuilder.append("'개까지 선택할 수 있습니다.");
                                Toast.makeText(context, stringBuilder.toString(), Toast.LENGTH_SHORT).show();

                                rhyaMusicInfoVOv2.setIsPlay(false);
                                notifyItemChanged(pos);

                                return;
                            }

                            stringBuilder.append(count + 1);
                            stringBuilder.append("개 선택됨");

                            songCount.setText(stringBuilder.toString());
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }
            });
        }

        public void onBind(RhyaMusicInfoVOv2 rhyaMusicInfoVOv2) {
            Glide.with(context)
                    .load(rhyaMusicInfoVOv2.getImage())
                    .placeholder(R.drawable.app_logo_for_black)
                    .error(R.drawable.app_logo_for_black)
                    .fallback(R.drawable.app_logo_for_black)
                    .into(imageView);

            if (rhyaMusicInfoVOv2.getIsPlay()) {
                itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.custom_gray_4));
            }else {
                itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.white));
            }

            name.setText(rhyaMusicInfoVOv2.getName());
            singer.setText(rhyaMusicInfoVOv2.getSinger());
        }
    }
}
