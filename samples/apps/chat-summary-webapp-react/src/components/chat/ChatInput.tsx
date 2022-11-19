// Copyright (c) Microsoft. All rights reserved.

import { Button, Input } from '@fluentui/react-components';
import { Send16Regular } from '@fluentui/react-icons';
import React, { FC } from 'react';
import { IChatMessage } from './ChatThread';

interface ChatInputProps {
    onSubmit: (message: IChatMessage) => void;
}

export const ChatInput: FC<ChatInputProps> = (props) => {
    const { onSubmit } = props;
