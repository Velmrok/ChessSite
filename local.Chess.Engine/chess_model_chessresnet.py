import torch
import torch.nn as nn


class ChessResNet(nn.Module):
    def __init__(self, in_channels=22, channels=64, num_blocks=8):
        super().__init__()
        self.stem = nn.Conv2d(in_channels, channels, kernel_size=3, padding=1)
        self.stem_bn = nn.BatchNorm2d(channels)

        self.blocks = nn.Sequential(
            *[ResidualBlock(channels) for _ in range(num_blocks)]
        )

        self.reduce = nn.Conv2d(channels, 16, kernel_size=1)
        self.fc1 = nn.Linear(16 * 8 * 8, 256)
        self.fc2 = nn.Linear(256, 64)
        self.fc3 = nn.Linear(64, 1)
        self.dropout = nn.Dropout(0.4)

        nn.init.xavier_uniform_(self.fc3.weight, gain=0.01)
        nn.init.zeros_(self.fc3.bias)

    def forward(self, x):
        x = torch.relu(self.stem_bn(self.stem(x)))
        x = self.blocks(x)
        x = torch.relu(self.reduce(x))
        x = x.view(x.size(0), -1)
        x = torch.relu(self.fc1(x))
        x = self.dropout(x)
        x = torch.relu(self.fc2(x))
        x = self.dropout(x)
        x = torch.tanh(self.fc3(x))
        return x
    

class ResidualBlock(nn.Module):
    def __init__(self, channels):
        super().__init__()
        self.conv1 = nn.Conv2d(channels, channels, kernel_size=3, padding=1)
        self.bn1 = nn.BatchNorm2d(channels)
        self.conv2 = nn.Conv2d(channels, channels, kernel_size=3, padding=1)
        self.bn2 = nn.BatchNorm2d(channels)

    def forward(self, x):
        identity = x
        out = torch.relu(self.bn1(self.conv1(x)))
        out = self.bn2(self.conv2(out))
        out = out + identity
        out = torch.relu(out)
        return out